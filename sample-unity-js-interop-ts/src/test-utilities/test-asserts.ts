
/// Collection of assert functions for testing

export function assertEqual<T>(actual: T, expected: T, testName: string): void {
    if (actual !== expected) {
        throw new Error(`Expected ${expected}, but got ${actual}`);
    }
    else {
        console.log(`Test named ${testName} passed ! Expected ${expected}, and got ${actual}`);
    }
}

export function assertArrayEqual<T>(actual: T[], expected: T[], testName: string): void {
    if (actual.length !== expected.length) {
        throw new Error(`Expected array length ${expected.length}, but got ${actual.length}`);
    }
    for (let i = 0; i < actual.length; i++) {
        if (actual[i] !== expected[i]) {
            throw new Error(`Expected ${expected[i]}, but got ${actual[i]}`);
        }
    }
    console.log(`Test named ${testName} passed ! Expected ${expected}, and got ${actual}`);
}

export function assertThrows(fn: () => void, testName: string): void {
    try {
        fn();
        throw new Error(`Expected an error, but no error was thrown.`);
    }
    catch (error) {
        console.log(`Test named ${testName} passed ! Expected an error, and got an error: ${error}`);
    }
}

export async function assertThrowsAsync(fn: () => Promise<void>, testName: string): Promise<void> {
    try {
        await fn();
        throw new Error(`Expected an error, but no error was thrown.`);
    }
    catch (error) {
        console.log(`Test named ${testName} passed ! Expected an error, and got an error: ${error}`);
    }
}

export function assertNotEqual<T>(actual: T, expected: T, testName: string): void {
    if (actual === expected) {
        throw new Error(`Expected ${expected} to be different from ${actual}.`);
    }
    else {
        console.log(`Test  named ${testName} passed ! Expected ${expected} to be different from ${actual}.`);
    }
}

export function assertTrue(value: any, testName: string): void {
    if (!value) {
        throw new Error(`Expected a truthy value, but got ${value}`); 
    }
    else {
        console.log(`Test named ${testName} passed ! Expected a truthy value, and got ${value}.`);
    }
}

export function assertFalse(value: any, testName: string): void {
    if (value) {
        throw new Error(`Expected a falsy value, but got ${value} !`)
    }
    else {
        console.log(`Test named ${testName} passed ! Expected a falsy value, and got ${value}.`);
    }
}
