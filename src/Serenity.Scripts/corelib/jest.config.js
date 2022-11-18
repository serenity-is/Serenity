const esbuildOptions = {}

export default {
    testEnvironment: 'jsdom',
    testMatch: ['<rootDir>/test/**/*.spec.ts'],
    moduleNameMapper: {
        '^@/(.*)$': '<rootDir>/src/$1',
        '^@serenity-is/corelib/q$': '<rootDir>/src/q',
        '^@serenity-is/corelib/slick$': '<rootDir>/src/slick'
    },
    "transform": {
        "^.+\\.tsx?$": ["jest-esbuild", esbuildOptions]
    }
};