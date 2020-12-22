module.exports = {
    preset: 'ts-jest',
    testEnvironment: 'node',
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    moduleNameMapper: {
        '^@CoreLib/(.*)$': '<rootDir>/CoreLib/$1',
        '^@Q/(.*)$': '<rootDir>/CoreLib/Q/$1'
    },
    globals: {
        'ts-jest': {
            tsconfig: 'test/tsconfig.json'
        }
    }
};