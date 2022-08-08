const esbuildOptions = {}

export default {
    testEnvironment: 'node',
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    moduleNameMapper: {
        '^@CoreLib/(.*)$': '<rootDir>/CoreLib/$1',
        '^@Q/(.*)$': '<rootDir>/CoreLib/Q/$1'
    },
    "transform": {
        "^.+\\.tsx?$": ["jest-esbuild", esbuildOptions]
    }
};