import { Authorization } from "./authorization";
import { ScriptData } from "./scriptdata";
import { UserDefinition } from "./userdefinition";
import * as notify from "./notify";
import * as localtext from "./localtext";

var userDefinition: UserDefinition;

function mockUserDefinition(async = false) {
    userDefinition = {
        IsAdmin: false,
        Username: "mockuser",
        DisplayName: "mockdisplay",
        Permissions: {
            "TRUE": true,
            "FALSE": false,
            "NO": false,
            "YES": true
        }
    }

    ScriptData.ensure = function (key: string) {
        if (async)
            throw new Error("not expected to call async ensure!");

        if (key == "RemoteData.UserData")
            return userDefinition as any;

        throw new Error("not expected to load any other data!");
    }

    ScriptData.ensureAsync = async function (key: string) {
        if (!async)
            throw new Error("not expected to call async ensure!");

        if (key == "RemoteData.UserData")
            return userDefinition as any;

        throw new Error("not expected to load any other data!");
    }
}

function mockUserDefinitionAsync() {
    mockUserDefinition(true);
}

function sharedPermissionSetBasedTests(testMethod: (permissionSet: { [key: string]: any }, permission: string) => Promise<boolean>) {

    it('returns false for null or undefined permission set', async function () {
        expect(await testMethod(null, 'test')).toBe(false);
        expect(await testMethod(undefined, 'test')).toBe(false);
    });

    it('returns false for null or undefined permission', async function () {
        expect(await testMethod({ x: true }, null)).toBe(false);
        expect(await testMethod({ x: true }, undefined)).toBe(false);
    });

    it('returns false if permission set does not contain the key', async function () {
        expect(await testMethod({ x: false }, "y")).toBe(false);
    });

    it('returns false if permission set contains the key with falsy value', async function () {
        expect(await testMethod({ x: false }, "x")).toBe(false);
        expect(await testMethod({ x: 0 as any }, "x")).toBe(false);
        expect(await testMethod({ x: '' as any }, "x")).toBe(false);
    });

    it('returns true if permission set contains the key with truthy value', async function () {
        expect(await testMethod({ x: true }, "x")).toBe(true);
        expect(await testMethod({ x: 1 as any }, "x")).toBe(true);
        expect(await testMethod({ x: "1" as any }, "x")).toBe(true);
    });

    it('returns false if one of & parts is not in the set', async function () {
        expect(await testMethod({ x: true, y: false }, "x&y")).toBe(false);
        expect(await testMethod({ x: 1 as any, y: 0 as any }, "x&y")).toBe(false);
        expect(await testMethod({ x: "2" as any, y: null }, "x&y")).toBe(false);
        expect(await testMethod({ x: true }, "x&y")).toBe(false);
    });

    it('returns false if one of & parts is not in the set', async function () {
        expect(await testMethod({ x: true, y: false }, "x&y")).toBe(false);
        expect(await testMethod({ x: 1 as any, y: 0 as any }, "x&y")).toBe(false);
        expect(await testMethod({ x: "2" as any, y: null }, "x&y")).toBe(false);
        expect(await testMethod({ x: true }, "x&y")).toBe(false);
    });

    it('returns true if both of & parts is truthy in the set', async function () {
        expect(await testMethod({ x: true, y: true }, "x&y")).toBe(true);
        expect(await testMethod({ x: 1 as any, y: 1 as any }, "x&y")).toBe(true);
        expect(await testMethod({ x: "2" as any, y: "3" as any }, "x&y")).toBe(true);
        expect(await testMethod({ x: 1 as any, y: true }, "x&y")).toBe(true);
    });

    it('returns true if both of | parts is truthy in the set', async function () {
        expect(await testMethod({ x: true, y: true }, "x|y")).toBe(true);
        expect(await testMethod({ x: 1 as any, y: 1 as any }, "x|y")).toBe(true);
        expect(await testMethod({ x: "2" as any, y: "3" as any }, "x|y")).toBe(true);
        expect(await testMethod({ x: 1 as any, y: true }, "x|y")).toBe(true);
    });

    it('returns true if one of | parts is truthy in the set', async function () {
        expect(await testMethod({ x: true, y: false }, "x|y")).toBe(true);
        expect(await testMethod({ x: null, y: 1 as any }, "x|y")).toBe(true);
        expect(await testMethod({ y: true }, "x|y")).toBe(true);
        expect(await testMethod({ x: true }, "x|y")).toBe(true);
    });

    it('assumes empty parts in double | to be false', async function () {
        expect(await testMethod({ x: true, y: true }, "x||y")).toBe(true);
        expect(await testMethod({ x: true, y: true }, "x||")).toBe(true);
        expect(await testMethod({ x: true, y: true }, "||x")).toBe(true);
        expect(await testMethod({ x: true, y: true }, "z||x")).toBe(true);
        expect(await testMethod({ x: true, y: true }, "u||w")).toBe(false);
        expect(await testMethod({ x: true, y: true }, "||w")).toBe(false);
        expect(await testMethod({ x: true, y: true }, "||")).toBe(false);
    });

    it('assumes empty parts with | are false', async function () {
        expect(await testMethod({ x: true, y: true }, "x|")).toBe(true);
        expect(await testMethod({ x: true }, "y|")).toBe(false);
        expect(await testMethod({}, "||")).toBe(false);
    });

    it('assumes empty parts with & to be false', async function () {
        expect(await testMethod({ x: true, y: true }, "x&")).toBe(false);
        expect(await testMethod({ x: true, y: true }, "&x")).toBe(false);
        expect(await testMethod({ x: true, y: true }, "y&x&")).toBe(false);
        expect(await testMethod({ x: true, y: true }, "&&")).toBe(false);
    });

    it('gives operator precedence to &', async function () {
        expect(await testMethod({ TRUE: true }, "TRUE&FALSE|TRUE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "TRUE&FALSE|FALSE")).toBe(false);
        expect(await testMethod({ TRUE: true }, "TRUE&TRUE|FALSE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "TRUE&TRUE|TRUE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "FALSE&FALSE|TRUE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "FALSE&FALSE|FALSE")).toBe(false);
        expect(await testMethod({ TRUE: true }, "FALSE&TRUE|FALSE")).toBe(false);
        expect(await testMethod({ TRUE: true }, "FALSE&TRUE|TRUE")).toBe(true);

        expect(await testMethod({ TRUE: true }, "TRUE|FALSE&TRUE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "TRUE|FALSE&FALSE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "TRUE|TRUE&FALSE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "TRUE|TRUE&TRUE")).toBe(true);
        expect(await testMethod({ TRUE: true }, "FALSE|FALSE&TRUE")).toBe(false);
        expect(await testMethod({ TRUE: true }, "FALSE|FALSE&FALSE")).toBe(false);
        expect(await testMethod({ TRUE: true }, "FALSE|TRUE&FALSE")).toBe(false);
        expect(await testMethod({ TRUE: true }, "FALSE|TRUE&TRUE")).toBe(true);
    });
}

function sharedHasPermissionTests(hasPermission: ((key: string) => Promise<boolean>)) {
    it('returns false if permission is null or undefined', async function () {
        expect(await hasPermission(null)).toBe(false);
        expect(await hasPermission(undefined)).toBe(false);
    });

    it('returns true if permission is *', async function () {
        expect(await hasPermission('*')).toBe(true);
    });

    it('returns false for null or undefined permission set', async function () {
        userDefinition.Permissions = null;
        expect(await hasPermission('test')).toBe(false);
        expect(await hasPermission('test')).toBe(false);
    });

    it('returns false if permission set does not contain the key', async function () {
        expect(await hasPermission("test")).toBe(false);
    });

    it('returns true for "?" if user is logged in', async function () {
        userDefinition.Username = 'username';
        expect(await hasPermission("?")).toBe(true);
    });

    it('returns false for "?" if user is NOT logged in', async function () {
        userDefinition.Username = '';
        expect(await hasPermission("?")).toBe(false);

        userDefinition.Username = null;
        expect(await hasPermission("?")).toBe(false);

        userDefinition = null;
        expect(await hasPermission("?")).toBe(false);

    });

    it('returns true for empty string if user is logged in', async function () {
        userDefinition.Username = 'username';
        expect(await hasPermission("")).toBe(true);
    });

    it('returns false for empty string if user is NOT logged in', async function () {
        userDefinition.Username = '';
        expect(await hasPermission("")).toBe(false);

        userDefinition.Username = null;
        expect(await hasPermission("")).toBe(false);

        userDefinition = null;
        expect(await hasPermission("")).toBe(false);

    });

    it('returns false if user definition is null', async function () {
        userDefinition = null;
        expect(await hasPermission("TRUE")).toBe(false);
        expect(await hasPermission("YES")).toBe(false);
    });

    it('returns true if user is super admin', async function () {
        userDefinition.IsAdmin = true;
        expect(await hasPermission("TRUE")).toBe(true);
        expect(await hasPermission("FALSE")).toBe(true);
        expect(await hasPermission("YES")).toBe(true);
        expect(await hasPermission("NO")).toBe(true);
        expect(await hasPermission("UNKNOWN")).toBe(true);
        expect(await hasPermission("!!!")).toBe(true);
        expect(await hasPermission("&&")).toBe(true);
        expect(await hasPermission("|")).toBe(true);
    });

    sharedPermissionSetBasedTests((permissionSet, permission) => {
        userDefinition.Permissions = permissionSet;
        return hasPermission(permission);
    });
}

describe('Authorization.hasPermission', () => {
    beforeEach(() => {
        mockUserDefinition();
    });

    sharedHasPermissionTests(async (permission) => Promise.resolve(Authorization.hasPermission(permission)));
});

describe('Authorization.hasPermissionAsync', () => {
    beforeEach(() => {
        mockUserDefinitionAsync();
    });

    sharedHasPermissionTests(Authorization.hasPermissionAsync);
});

describe('Authorization.isPermissionInSet', () => {
    sharedPermissionSetBasedTests(async (permissionSet, permission) => Promise.resolve(Authorization.isPermissionInSet(permissionSet, permission)));
});


describe('Authorization.isLoggedIn', () => {
    beforeEach(() => {
        mockUserDefinition();
    })

    it('returns false if userdefinition is null', () => {
        userDefinition = null;
        expect(Authorization.isLoggedIn).toBe(false);
    });

    it('returns false if username is null or empty string', () => {
        userDefinition.Username = null;
        expect(Authorization.isLoggedIn).toBe(false);
        userDefinition.Username = "";
        expect(Authorization.isLoggedIn).toBe(false);
    });

    it('returns true if the username from userdefinition not null or an empty string', () => {
        userDefinition.Username = 'x';
        expect(Authorization.isLoggedIn).toBe(true);
        userDefinition.Username = '0' as any;
        expect(Authorization.isLoggedIn).toBe(true);
    });
});


describe('Authorization.isLoggedInAsync', () => {
    beforeEach(() => {
        mockUserDefinitionAsync();
    })

    it('returns false if userdefinition is null', async () => {
        userDefinition = null;
        expect(await Authorization.isLoggedInAsync).toBe(false);
    });

    it('returns false if username is null or empty string', async () => {
        userDefinition.Username = null;
        expect(await Authorization.isLoggedInAsync).toBe(false);
        userDefinition.Username = "";
        expect(await Authorization.isLoggedInAsync).toBe(false);
    });

    it('returns true if the username from userdefinition not null or an empty string', async () => {
        userDefinition.Username = 'x';
        expect(await Authorization.isLoggedInAsync).toBe(true);
        userDefinition.Username = '0' as any;
        expect(await Authorization.isLoggedInAsync).toBe(true);
    });
});

describe('Authorization.username', () => {
    beforeEach(() => {
        mockUserDefinition();
    })

    it('returns undefined if userdefinition is null', () => {
        userDefinition = null;
        expect(Authorization.username).toBeUndefined();
    });

    it('returns the username from userdefinition is null', () => {
        userDefinition.Username = 'x';
        expect(Authorization.username).toBe("x");
        userDefinition.Username = "";
        expect(Authorization.username).toBe("");
        userDefinition.Username = null;
        expect(Authorization.username).toBe(null);
    });
});

describe('Authorization.usernameAsync()', () => {
    beforeEach(() => {
        mockUserDefinitionAsync();
    })

    it('returns undefined if userdefinition is null', async () => {
        userDefinition = null;
        expect(await Authorization.usernameAsync).toBeUndefined();
    });

    it('returns the username from userdefinition is null', async () => {
        userDefinition.Username = 'x';
        expect(await Authorization.usernameAsync).toBe("x");
        userDefinition.Username = "";
        expect(await  Authorization.usernameAsync).toBe("");
        userDefinition.Username = null;
        expect(await Authorization.usernameAsync).toBe(null);
    });
});

describe('Authorization.userDefinition', () => {
    beforeEach(() => {
        mockUserDefinition();
    })

    it('returns null if userDefinition is null', () => {
        userDefinition = null;
        expect(Authorization.userDefinition).toBeNull();
    });

    it('returns the userDefinition as is', () => {
        expect(Authorization.userDefinition === userDefinition).toBe(true);
    });
});

describe('Authorization.userDefinitionAsync', () => {
    beforeEach(() => {
        mockUserDefinitionAsync();
    })

    it('returns null if userDefinition is null', async () => {
        userDefinition = null;
        expect(await Authorization.userDefinitionAsync).toBeNull();
    });

    it('returns the userDefinition as is', async () => {
        expect(await Authorization.userDefinitionAsync === userDefinition).toBe(true);
    });
});

describe('Authorization.validatePermission', () => {
    beforeEach(() => {
        mockUserDefinition();
    })

    it('throws if no permission', function () {
        var thrown = false;
        try {
            Authorization.validatePermission("FALSE");
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('AccessDenied') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

    it('does not throw if have the permission', function () {
        var thrown = false;
        try {
            Authorization.validatePermission("TRUE");
        }
        catch (e) {
            thrown = true;
        }
        expect(thrown).toBe(false);
    });
});

describe('Authorization.validatePermissionAsync', () => {
    beforeEach(() => {
        mockUserDefinitionAsync();
    })

    it('throws if no permission', async function () {
        const notifyError = jest.spyOn(notify, 'notifyError');
        const localText = jest.spyOn(localtext, 'localText');
        try {
            var thrown = false;
            try {
                await Authorization.validatePermissionAsync("FALSE");
            }
            catch (e) {
                thrown = true;
                expect(e.toString().indexOf('AccessDenied') >= 0).toBe(true);
            }
            expect(thrown).toBe(true);
            expect(localText.mock.calls.length).toBe(2);
            expect(localText.mock.calls[0]).toEqual(["Authorization.AccessDenied"]);
            expect(localText.mock.calls[1]).toEqual(["Authorization.AccessDenied"]);
            expect(notifyError.mock.calls.length).toBe(1);
            expect(notifyError.mock.calls[0]).toEqual(["Authorization.AccessDenied"]);
        }
        finally {
            notifyError.mockRestore();
            localText.mockRestore();
        }
    });

    it('does not throw if have the permission', async function () {
        var thrown = false;
        try {
            await Authorization.validatePermissionAsync("TRUE");
        }
        catch (e) {
            thrown = true;
        }
        expect(thrown).toBe(false);
    });
});