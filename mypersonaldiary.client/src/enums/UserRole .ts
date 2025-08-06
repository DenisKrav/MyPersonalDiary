export const UserRole = {
    User: "User",
    Admin: "Admin",
} as const;

export type UserRole = (typeof UserRole)[keyof typeof UserRole];