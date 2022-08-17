const esbuildOptions = {}

export default {
    testEnvironment: 'jsdom',
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    moduleNameMapper: {
        '^@/(.*)$': '<rootDir>/src/$1',
    },
    "transform": {
        "^.+\\.tsx?$": ["jest-esbuild", esbuildOptions]
    }
};