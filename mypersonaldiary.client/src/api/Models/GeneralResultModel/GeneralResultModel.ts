export interface GeneralResultModel<T = unknown> {
    errors: string[];
    result: T | null;
    hasErrors: boolean;
}
