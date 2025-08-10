export const NoteType = {
    Text: "Text",
    Image: "Image",
} as const;

export type NoteType = (typeof NoteType)[keyof typeof NoteType];