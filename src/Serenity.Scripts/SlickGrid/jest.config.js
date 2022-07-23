module.exports = {
    testEnvironment: "@happy-dom/jest-environment",
    testEnvironmentOptions: {
        resources: "usable",
        runScripts: "dangerously"
    },
    setupFilesAfterEnv: ["<rootDir>/jest.setup.js"],
    testMatch: ['<rootDir>/test/**/*.spec.js']
};