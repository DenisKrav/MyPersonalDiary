import type { NoteType } from "../../../enums/NoteType";

export interface DeleteNoteModel {
        noteType: NoteType;
        noteId: string;
}