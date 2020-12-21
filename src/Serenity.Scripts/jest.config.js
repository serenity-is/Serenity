module.exports = {
    preset: 'ts-jest',
    testEnvironment: 'node',
    rootDir: '.',
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    moduleNameMapper: {
        "^jQuery$": "<rootDir>/node_modules/jquery/dist/jquery.js",
        "^SerenityCoreLib$": "<rootDir>/dist/Serenity.CoreLib.js"
    }
};