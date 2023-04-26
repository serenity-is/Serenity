import { Authorization } from "./authorization";
import { ScriptData } from "./scriptdata";
import { UserDefinition } from "./userdefinition";

var userDefinition: UserDefinition;

function mockUserDefinition(async: boolean) {
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

function sharedPermissionSetBasedTests(testMethod: (permissionSet: { [key: string]: any }, permission: string) => Promise<boolean>) {

    it('returns false for null or undefined permission set', async function () {
        expect((await testMethod(null, 'test'))).toBe(false);
        expect((await testMethod(undefined, 'test'))).toBe(false);
    });

    it('returns false for null or undefined permission', async function () {
        expect((await testMethod({ x: true }, null))).toBe(false);
        expect((await testMethod({ x: true }, undefined))).toBe(false);
    });

    it('returns false if permission set does not contain the key', async function () {
        expect((await testMethod({ x: false }, "y"))).toBe(false);
    });

    it('returns false if permission set contains the key with falsy value', async function () {
        expect((await testMethod({ x: false }, "x"))).toBe(false);
        expect((await testMethod({ x: 0 as any }, "x"))).toBe(false);
        expect((await testMethod({ x: '' as any }, "x"))).toBe(false);
    });

    it('returns true if permission set contains the key with truthy value', async function () {
        expect((await testMethod({ x: true }, "x"))).toBe(true);
        expect((await testMethod({ x: 1 as any }, "x"))).toBe(true);
        expect((await testMethod({ x: "1" as any }, "x"))).toBe(true);
    });

    it('returns false if one of & parts is not in the set', async function () {
        expect((await testMethod({ x: true, y: false }, "x&y"))).toBe(false);
        expect((await testMethod({ x: 1 as any, y: 0 as any }, "x&y"))).toBe(false);
        expect((await testMethod({ x: "2" as any, y: null }, "x&y"))).toBe(false);
        expect((await testMethod({ x: true }, "x&y"))).toBe(false);
    });

    it('returns false if one of & parts is not in the set', async function () {
        expect((await testMethod({ x: true, y: false }, "x&y"))).toBe(false);
        expect((await testMethod({ x: 1 as any, y: 0 as any }, "x&y"))).toBe(false);
        expect((await testMethod({ x: "2" as any, y: null }, "x&y"))).toBe(false);
        expect((await testMethod({ x: true }, "x&y"))).toBe(false);
    });

    it('returns true if both of & parts is truthy in the set', async function () {
        expect((await testMethod({ x: true, y: true }, "x&y"))).toBe(true);
        expect((await testMethod({ x: 1 as any, y: 1 as any }, "x&y"))).toBe(true);
        expect((await testMethod({ x: "2" as any, y: "3" as any }, "x&y"))).toBe(true);
        expect((await testMethod({ x: 1 as any, y: true }, "x&y"))).toBe(true);
    });

    it('returns true if both of | parts is truthy in the set', async function () {
        expect((await testMethod({ x: true, y: true }, "x|y"))).toBe(true);
        expect((await testMethod({ x: 1 as any, y: 1 as any }, "x|y"))).toBe(true);
        expect((await testMethod({ x: "2" as any, y: "3" as any }, "x|y"))).toBe(true);
        expect((await testMethod({ x: 1 as any, y: true }, "x|y"))).toBe(true);
    });

    it('returns true if one of | parts is truthy in the set', async function () {
        expect((await testMethod({ x: true, y: false }, "x|y"))).toBe(true);
        expect((await testMethod({ x: null, y: 1 as any }, "x|y"))).toBe(true);
        expect((await testMethod({ y: true }, "x|y"))).toBe(true);
        expect((await testMethod({ x: true }, "x|y"))).toBe(true);
    });

    it('assumes empty parts in double | to be false', async function () {
        expect((await testMethod({ x: true, y: true }, "x||y"))).toBe(true);
        expect((await testMethod({ x: true, y: true }, "x||"))).toBe(true);
        expect((await testMethod({ x: true, y: true }, "||x"))).toBe(true);
        expect((await testMethod({ x: true, y: true }, "z||x"))).toBe(true);
        expect((await testMethod({ x: true, y: true }, "u||w"))).toBe(false);
        expect((await testMethod({ x: true, y: true }, "||w"))).toBe(false);
        expect((await testMethod({ x: true, y: true }, "||"))).toBe(false);
    });

    it('assumes empty parts with | are false', async function () {
        expect((await testMethod({ x: true, y: true }, "x|"))).toBe(true);
        expect((await testMethod({ x: true }, "y|"))).toBe(false);
        expect((await testMethod({}, "||"))).toBe(false);
    });

    it('assumes empty parts with & to be false', async function () {
        expect((await testMethod({ x: true, y: true }, "x&"))).toBe(false);
        expect((await testMethod({ x: true, y: true }, "&x"))).toBe(false);
        expect((await testMethod({ x: true, y: true }, "y&x&"))).toBe(false);
        expect((await testMethod({ x: true, y: true }, "&&"))).toBe(false);
    });

    it('gives operator precedence to &', async function () {
        expect((await testMethod({ TRUE: true }, "TRUE&FALSE|TRUE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "TRUE&FALSE|FALSE"))).toBe(false);
        expect((await testMethod({ TRUE: true }, "TRUE&TRUE|FALSE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "TRUE&TRUE|TRUE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "FALSE&FALSE|TRUE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "FALSE&FALSE|FALSE"))).toBe(false);
        expect((await testMethod({ TRUE: true }, "FALSE&TRUE|FALSE"))).toBe(false);
        expect((await testMethod({ TRUE: true }, "FALSE&TRUE|TRUE"))).toBe(true);

        expect((await testMethod({ TRUE: true }, "TRUE|FALSE&TRUE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "TRUE|FALSE&FALSE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "TRUE|TRUE&FALSE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "TRUE|TRUE&TRUE"))).toBe(true);
        expect((await testMethod({ TRUE: true }, "FALSE|FALSE&TRUE"))).toBe(false);
        expect((await testMethod({ TRUE: true }, "FALSE|FALSE&FALSE"))).toBe(false);
        expect((await testMethod({ TRUE: true }, "FALSE|TRUE&FALSE"))).toBe(false);
        expect((await testMethod({ TRUE: true }, "FALSE|TRUE&TRUE"))).toBe(true);
    });
}

function sharedHasPermissionTests(hasPermission: ((key: string) => Promise<boolean>)) {
    it('returns false if permission is null or undefined', async function () {
        expect((await hasPermission(null))).toBe(false);
        expect((await hasPermission(undefined))).toBe(false);
    });

    it('returns true if permission is *', async function () {
        expect((await hasPermission('*'))).toBe(true);
    });

    it('returns false for null or undefined permission set', async function () {
        userDefinition.Permissions = null;
        expect((await hasPermission('test'))).toBe(false);
        expect((await hasPermission('test'))).toBe(false);
    });

    it('returns false if permission set does not contain the key', async function () {
        expect((await hasPermission("test"))).toBe(false);
    });

    it('returns true for "?" if user is logged in', async function () {
        userDefinition.Username = 'username';
        expect((await hasPermission("?"))).toBe(true);
    });

    it('returns false for "?" if user is NOT logged in', async function () {
        userDefinition.Username = '';
        expect((await hasPermission("?"))).toBe(false);

        userDefinition.Username = null;
        expect((await hasPermission("?"))).toBe(false);

        userDefinition = null;
        expect((await hasPermission("?"))).toBe(false);

    });

    it('returns true for empty string if user is logged in', async function () {
        userDefinition.Username = 'username';
        expect((await hasPermission(""))).toBe(true);
    });

    it('returns false for empty string if user is NOT logged in', async function () {
        userDefinition.Username = '';
        expect((await hasPermission(""))).toBe(false);

        userDefinition.Username = null;
        expect((await hasPermission(""))).toBe(false);

        userDefinition = null;
        expect((await hasPermission(""))).toBe(false);

    });

    it('returns false if user definition is null', async function () {
        userDefinition = null;
        expect((await hasPermission("TRUE"))).toBe(false);
        expect((await hasPermission("YES"))).toBe(false);
    });

    it('returns true if user is super admin', async function () {
        userDefinition.IsAdmin = true;
        expect((await hasPermission("TRUE"))).toBe(true);
        expect((await hasPermission("FALSE"))).toBe(true);
        expect((await hasPermission("YES"))).toBe(true);
        expect((await hasPermission("NO"))).toBe(true);
        expect((await hasPermission("UNKNOWN"))).toBe(true);
        expect((await hasPermission("!!!"))).toBe(true);
        expect((await hasPermission("&&"))).toBe(true);
        expect((await hasPermission("|"))).toBe(true);
    });

    sharedPermissionSetBasedTests((permissionSet, permission) => {
        userDefinition.Permissions = permissionSet;
        return hasPermission(permission);
    });
}

describe('Authorization.hasPermission', () => {
    beforeEach(() => {
        mockUserDefinition(false); // sync
    });
    
    sharedHasPermissionTests(async (permission) => Promise.resolve(Authorization.hasPermission(permission)));
});

describe('Authorization.hasPermissionAsync', () => {
    beforeEach(() => {
        mockUserDefinition(true); // async
    });

    sharedHasPermissionTests(Authorization.hasPermissionAsync);
});

describe('Authorization.isPermissionInSet', () => {
    sharedPermissionSetBasedTests(async (permissionSet, permission) => Promise.resolve(Authorization.isPermissionInSet(permissionSet, permission)));
});
