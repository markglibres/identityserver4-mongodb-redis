export const getFormikError = <T>(errors: T, keyName: keyof T) =>
    errors && errors[keyName] ? ((errors[keyName] as unknown) as string) : '';
