import { type UserDefinition } from "./userdefinition";
import { Authorization } from "./authorization";

const baseUser: UserDefinition = {
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

const { getUserDefinitionMock } = vi.hoisted(() => {
    return {
        getUserDefinitionMock: vi.fn()
    };
});

vi.mock("./notify", () => ({
    notifyError: vi.fn()
}));

vi.mock("./localtext", () => ({
    localText: vi.fn().mockImplementation((key: string) => key)
}));

vi.mock(import("./scriptdata"), async () => {
    return {
        getRemoteData: vi.fn().mockImplementation((key: string) => {
            if (key == "UserData")
                return getUserDefinitionMock();

            throw new Error(`not expected to load any other data: ${key}`);
        }),
        getRemoteDataAsync: vi.fn().mockImplementation(async (key: string) => {
            if (key == "UserData")
                return getUserDefinitionMock();

            throw new Error(`not expected to load any other data: ${key}`);
        })
    }
});

function mockUserDefinition(user?: Partial<UserDefinition>): UserDefinition {
    if (user === null) {
        getUserDefinitionMock.mockReturnValue(null);
        return null;
    }

    user = {
        ...baseUser,
        ...user
    };
    getUserDefinitionMock.mockReturnValue(user);
    return user;
}

beforeEach(() => {
    vitest.clearAllMocks();
    mockUserDefinition();
})

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

describe('Authorization.hasPermission', () => {
    it('returns false if permission is null or undefined', async function () {
        expect(Authorization.hasPermission(null)).toBe(false);
        expect(Authorization.hasPermission(undefined)).toBe(false);
    });

    it('returns true if permission is *', async function () {
        expect(Authorization.hasPermission('*')).toBe(true);
    });

    it('returns false for null or undefined permission set', async function () {
        mockUserDefinition({ Permissions: null });
        expect(Authorization.hasPermission('test')).toBe(false);
        expect(Authorization.hasPermission('test')).toBe(false);
    });

    it('returns false if permission set does not contain the key', async function () {
        expect(Authorization.hasPermission("test")).toBe(false);
    });

    it('returns true for "?" if user is logged in', async function () {
        mockUserDefinition({ Username: 'username' });
        expect(Authorization.hasPermission("?")).toBe(true);
    });

    it('returns false for "?" if user is NOT logged in', async function () {
        mockUserDefinition({ Username: '' });
        expect(Authorization.hasPermission("?")).toBe(false);

        mockUserDefinition({ Username:  null });
        expect(Authorization.hasPermission("?")).toBe(false);

        mockUserDefinition(null);
        expect(Authorization.hasPermission("?")).toBe(false);

    });

    it('returns true for empty string if user is logged in', async function () {
        mockUserDefinition({ Username: 'username' });
        expect(Authorization.hasPermission("")).toBe(true);
    });

    it('returns false for empty string if user is NOT logged in', async function () {
        mockUserDefinition({ Username: '' });
        expect(Authorization.hasPermission("")).toBe(false);

        mockUserDefinition({ Username: null });
        expect(Authorization.hasPermission("")).toBe(false);

        mockUserDefinition(null);;
        expect(Authorization.hasPermission("")).toBe(false);

    });

    it('returns false if user definition is null', async function () {
        mockUserDefinition(null);;
        expect(Authorization.hasPermission("TRUE")).toBe(false);
        expect(Authorization.hasPermission("YES")).toBe(false);
    });

    it('returns true if user is super admin', async function () {
        mockUserDefinition({ IsAdmin: true });
        expect(Authorization.hasPermission("TRUE")).toBe(true);
        expect(Authorization.hasPermission("FALSE")).toBe(true);
        expect(Authorization.hasPermission("YES")).toBe(true);
        expect(Authorization.hasPermission("NO")).toBe(true);
        expect(Authorization.hasPermission("UNKNOWN")).toBe(true);
        expect(Authorization.hasPermission("!!!")).toBe(true);
        expect(Authorization.hasPermission("&&")).toBe(true);
        expect(Authorization.hasPermission("|")).toBe(true);
    });

    //sharedPermissionSetBasedTests((permissionSet, permission) => {
    //    mockUserDefinition({ Permissions: permissionSet });
    //    return Authorization.hasPermission(permission);
    //});
    

});

describe('Authorization.hasPermissionAsync', () => {
    it('returns false if permission is null or undefined', async function () {
        expect(await Authorization.hasPermissionAsync(null)).toBe(false);
        expect(await Authorization.hasPermissionAsync(undefined)).toBe(false);
    });

    it('returns true if permission is *', async function () {
        expect(await Authorization.hasPermissionAsync('*')).toBe(true);
    });

    it('returns false for null or undefined permission set', async function () {
        mockUserDefinition({ Permissions: null });
        expect(await Authorization.hasPermissionAsync('test')).toBe(false);
        expect(await Authorization.hasPermissionAsync('test')).toBe(false);
    });

    it('returns false if permission set does not contain the key', async function () {
        expect(await Authorization.hasPermissionAsync("test")).toBe(false);
    });

    it('returns true for "?" if user is logged in', async function () {
        mockUserDefinition({ Username: 'username' });
        expect(await Authorization.hasPermissionAsync("?")).toBe(true);
    });

    it('returns false for "?" if user is NOT logged in', async function () {
        mockUserDefinition({ Username: '' });
        expect(await Authorization.hasPermissionAsync("?")).toBe(false);

        mockUserDefinition({ Username:  null });
        expect(await Authorization.hasPermissionAsync("?")).toBe(false);

        mockUserDefinition(null);
        expect(await Authorization.hasPermissionAsync("?")).toBe(false);

    });

    it('returns true for empty string if user is logged in', async function () {
        mockUserDefinition({ Username: 'username' });
        expect(await Authorization.hasPermissionAsync("")).toBe(true);
    });

    it('returns false for empty string if user is NOT logged in', async function () {
        mockUserDefinition({ Username: '' });
        expect(await Authorization.hasPermissionAsync("")).toBe(false);

        mockUserDefinition({ Username: null });
        expect(await Authorization.hasPermissionAsync("")).toBe(false);

        mockUserDefinition(null);;
        expect(await Authorization.hasPermissionAsync("")).toBe(false);

    });

    it('returns false if user definition is null', async function () {
        mockUserDefinition(null);;
        expect(await Authorization.hasPermissionAsync("TRUE")).toBe(false);
        expect(await Authorization.hasPermissionAsync("YES")).toBe(false);
    });

    it('returns true if user is super admin', async function () {
        mockUserDefinition({ IsAdmin: true });
        expect(await Authorization.hasPermissionAsync("TRUE")).toBe(true);
        expect(await Authorization.hasPermissionAsync("FALSE")).toBe(true);
        expect(await Authorization.hasPermissionAsync("YES")).toBe(true);
        expect(await Authorization.hasPermissionAsync("NO")).toBe(true);
        expect(await Authorization.hasPermissionAsync("UNKNOWN")).toBe(true);
        expect(await Authorization.hasPermissionAsync("!!!")).toBe(true);
        expect(await Authorization.hasPermissionAsync("&&")).toBe(true);
        expect(await Authorization.hasPermissionAsync("|")).toBe(true);
    });

    sharedPermissionSetBasedTests((permissionSet, permission) => {
        mockUserDefinition({ Permissions: permissionSet });
        return Authorization.hasPermissionAsync(permission);
    });
});

describe('Authorization.isPermissionInSet', () => {
    sharedPermissionSetBasedTests(async (permissionSet, permission) => Promise.resolve(Authorization.isPermissionInSet(permissionSet, permission)));
});


describe('Authorization.isLoggedIn', () => {
    beforeEach(() => {
        mockUserDefinition();
    })

    it('returns false if userdefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition(null);;
        expect(authorization.isLoggedIn).toBe(false);
    });

    it('returns false if username is null or empty string', async () => {
        let authorization = Authorization;
        mockUserDefinition({ Username: null });
        expect(authorization.isLoggedIn).toBe(false);
        mockUserDefinition({ Username: "" });
        expect(authorization.isLoggedIn).toBe(false);
    });

    it('returns true if the username from userdefinition not null or an empty string', async () => {
        let authorization = Authorization;
        mockUserDefinition({ Username: 'x' });
        expect(authorization.isLoggedIn).toBe(true);
        mockUserDefinition({ Username: '0' as any });
        expect(authorization.isLoggedIn).toBe(true);
    });
});


describe('Authorization.isLoggedInAsync', () => {
    
    it('returns false if userdefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition(null);;
        expect(await authorization.isLoggedInAsync).toBe(false);
    });

    it('returns false if username is null or empty string', async () => {
        let authorization = Authorization;
        mockUserDefinition({ Username: null });
        expect(await authorization.isLoggedInAsync).toBe(false);
        mockUserDefinition({ Username: "" });
        expect(await authorization.isLoggedInAsync).toBe(false);
    });

    it('returns true if the username from userdefinition not null or an empty string', async () => {
        let authorization = Authorization;
        mockUserDefinition({ Username: 'x' });
        expect(await authorization.isLoggedInAsync).toBe(true);
        mockUserDefinition({ Username: '0' as any });
        expect(await authorization.isLoggedInAsync).toBe(true);
    });
});

describe('Authorization.username', () => {
    it('returns undefined if userdefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition(null);;
        expect(authorization.username).toBeUndefined();
    });

    it('returns the username from userdefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition({ Username: 'x' });
        expect(authorization.username).toBe("x");
        mockUserDefinition({ Username: "" });
        expect(authorization.username).toBe("");
        mockUserDefinition({ Username: null });
        expect(authorization.username).toBe(null);
    });
});

describe('Authorization.usernameAsync()', () => {

    it('returns undefined if userdefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition(null);;
        expect(await authorization.usernameAsync).toBeUndefined();
    });

    it('returns the username from userdefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition({ Username: 'x' });
        expect(await authorization.usernameAsync).toBe("x");
        mockUserDefinition({ Username: "" });
        expect(await authorization.usernameAsync).toBe("");
        mockUserDefinition({ Username: null });
        expect(await authorization.usernameAsync).toBe(null);
    });
});

describe('Authorization.userDefinition', () => {

    it('returns null if userDefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition(null);;
        expect(authorization.userDefinition).toBeNull();
    });

    it('returns the userDefinition as is', async () => {
        let authorization = Authorization;
        let userDefinition = mockUserDefinition();
        expect(authorization.userDefinition === userDefinition).toBe(true);
    });
});

describe('Authorization.userDefinitionAsync', () => {
    
    it('returns null if userDefinition is null', async () => {
        let authorization = Authorization;
        mockUserDefinition(null);;
        expect(await authorization.userDefinitionAsync).toBeNull();
    });

    it('returns the userDefinition as is', async () => {
        let authorization = Authorization;
        let userDefinition = mockUserDefinition();
        expect(await (authorization.userDefinitionAsync) === userDefinition).toBe(true);
    });
});

describe('Authorization.validatePermission', () => {

    it('throws if no permission', async function () {
        let authorization = Authorization;
        var thrown = false;
        try {
            authorization.validatePermission("FALSE");
        }
        catch (e) {
            thrown = true;
            expect(e.toString().indexOf('AccessDenied') >= 0).toBe(true);
        }
        expect(thrown).toBe(true);
    });

    it('does not throw if have the permission', async function () {
        let authorization = Authorization;
        var thrown = false;
        try {
            authorization.validatePermission("TRUE");
        }
        catch (e) {
            thrown = true;
        }
        expect(thrown).toBe(false);
    });
});

describe('Authorization.validatePermissionAsync', () => {
    it('throws if no permission', async function () {
        const notify = await import("./notify") as any;
        const localText = (await import("./localtext")).localText as any;
        const notifyError = notify.notifyError as any;
    let authorization = Authorization;
        try {
            var thrown = false;
            try {
                await authorization.validatePermissionAsync("FALSE");
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
        let authorization = Authorization;
        var thrown = false;
        try {
            await authorization.validatePermissionAsync("TRUE");
        }
        catch (e) {
            thrown = true;
        }
        expect(thrown).toBe(false);
    });
});