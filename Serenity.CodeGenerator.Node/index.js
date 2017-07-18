#! /usr/bin/env node

var inquirer = require('inquirer');
inquirer.registerPrompt('autocomplete', require('inquirer-autocomplete-prompt'));
var fs = require('fs');
var path = require('path');
var temp = require('temp').track();

var generate = 'Generate Code for a Table';
var restore = 'Restore Static Content from NuGet Packages';
var transform = 'Transform ALL (Client Types / MVC / Server Typings)';
var clientTypes = 'Transform Client Types';
var mvc = 'Transform MVC';
var serverTypings = 'Transform Server Typings';

var cmd = null;
if (process.argv.length > 2) {
    cmd = process.argv[2].toLowerCase();
    if ('generate'.startsWith(cmd))
        cmd = generate;
    else if ('restore'.startsWith(cmd))
        cmd = restore;
    else if ('transform'.startsWith(cmd))
        cmd = transform;
    else if ('clienttypes'.startsWith(cmd))
        cmd = clientTypes;
    else if ('mvc'.startsWith(cmd))
        cmd = mvc;
    else if ('serverTypings'.startsWith(cmd))
        cmd = serverTypings;
}

if (!fs.existsSync('./sergen.json')) {
    console.error('Please run Sergen in a directory containing Serenity ASP.NET Core project!');
    process.exit(2);
}

//var projectJson = JSON.parse(fs.readFileSync('./project.json', 'utf8').replace(/^\uFEFF/, ''));
//if (!projectJson || !projectJson.tools || (projectJson.tools['Serenity.CodeGenerator'] == null)) {
//    console.error('"Serenity.CodeGenerator" package is not found in your project.json "tools" section!');
//    process.exit(3);
//}

var spawn = require('child_process').spawn;
var generateOptions = {};

function runGenerate() {
    
    if (generateOptions.connection == null) {
        var tempFile = temp.path() + '.json';
        spawn("dotnet", ["sergen", "g", "-o", tempFile], {stdio: "inherit"}).on('exit', function(code) {
            if (code != 0)
                process.exit(code);

            generateOptions.connections = JSON.parse(fs.readFileSync(tempFile, 'utf8').replace(/^\uFEFF/, ''));
            inquirer.prompt({
                message: 'Select a Connection:',
                type: 'list',
                name: 'connection',
                choices: generateOptions.connections
            }).then(function(answers) {
                generateOptions.connection = answers.connection;
                runGenerate();
            }).catch(function(err) {
                console.log(err);
                process.exit(4);
            });
        });

        return;
    }

    if (generateOptions.table == null) {
        var tempFile = temp.path() + '.json';
        spawn("dotnet", ["sergen", "g", "-o", tempFile, "-c", generateOptions.connection], {stdio: "inherit"}).on('exit', function(code) {
            if (code != 0)
                process.exit(code);

            generateOptions.tables = JSON.parse(fs.readFileSync(tempFile, 'utf8').replace(/^\uFEFF/, ''));

            var tableNames = generateOptions.tables.map(function(x) { return x.name });
            function searchTables(answers, input) {
                return new Promise(function(resolve) {
                    resolve(tableNames.filter(function(u) {
                        return new RegExp(input || '', 'i').exec(u) !== null;
                    }));
                });
            }

            inquirer.prompt({
                message: 'Select a Table:',
                type: 'autocomplete',
                name: 'table',
                source: searchTables
            }).then(function(answers) {
                generateOptions.table = answers.table;
                runGenerate();
            }).catch(function(err) {
                console.log(err);
                process.exit(5);
            });
        });

        return;        
    }

    var tbl = generateOptions.tables.filter(x => x.name == generateOptions.table)[0];
    inquirer.prompt([{
        message: 'Module Name:',
        type: 'input',
        name: 'module',
        default: function() { return tbl.module; }
    }, {
        message: 'Identifier:',
        type: 'input',
        name: 'identifier',
        default: function() { return tbl.identifier; }
    }, {
        message: 'Permission Key:',
        type: 'input',
        name: 'permission',
        default: function() { return tbl.permission; }
    }, {
        message: 'Choose What to Generate:',
        type: 'checkbox',
        name: 'what',
        choices: [{
            name: 'Row (Entity)',
            checked: true
        }, {
            name: 'Services (Repository, Endpoint)',
            checked: true
        }, {
            name: 'UI (Columns, Form, Page, Grid, Dialog, Css)',
            checked: true
        }]
    }]).then(function(answers) {
        spawn("dotnet", [
            "sergen", "g", 
            "-c", generateOptions.connection,
            "-t", generateOptions.table,
            "-m", answers.module,
            "-i", answers.identifier,
            "-p", answers.permission,
            "-w", answers.what.map(x => x.charAt(0)).join('')
        ], { stdio: "inherit" }).on('exit', function(code) {
            process.exit(code);
        });

    }).catch(function(err) {
        console.log(err);
        process.exit(6);
    });        
}

function runCommand(cmd) {
    switch (cmd) {
        case generate:
            runGenerate();
            break;

        case restore:
            spawn("dotnet", ["sergen", "restore"], {stdio: "inherit"}).on('exit', function(code) {
                process.exit(code);
            });
            break;

        case transform:
            spawn("dotnet", ["sergen", "transform"], {stdio: "inherit"}).on('exit', function(code) {
                process.exit(code);
            });
            break;

        case clientTypes:
            spawn("dotnet", ["sergen", "clienttypes"], {stdio: "inherit"}).on('exit', function(code) {
                process.exit(code);
            });
            break;

        case serverTypings:
            spawn("dotnet", ["sergen", "servertypings"], {stdio: "inherit"}).on('exit', function(code) {
                process.exit(code);
            });
            break;  

        case mvc:
            spawn("dotnet", ["sergen", "mvc"], {stdio: "inherit"}).on('exit', function(code) {
                process.exit(code);
            });
            break;       

        default: 
            console.error('Unknown Command: ' + cmd);
            process.exit(1);       
            break;        
    }
}

if (!cmd) {
    inquirer.prompt({
        message: 'What can i do for you?',
        type: 'list',
        name: 'command',
        choices: [
            generate,
            transform,
            clientTypes,
            mvc,
            serverTypings,
            restore
        ]
    }).then(function(answers) {
        runCommand(answers.command);
    }).catch(function(err) {
        console.log(err);
    });
}
else {
    runCommand(cmd);
}