module.exports = {
    preset: 'ts-jest',
    testEnvironment: "jsdom",
    testEnvironmentOptions: {
        resources: "usable"
    },
    setupFilesAfterEnv: ["<rootDir>/jest.setup.js"],
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    globals: {
        'ts-jest': {
            tsconfig: '<rootDir>/test/tsconfig.json'
        }
    }
};