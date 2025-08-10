import axios from "axios";
import type { GeneralResultModel } from "../Models/GeneralResultModel/GeneralResultModel";
import type { NoteModel } from "../Models/Note/NoteModel";
import type { NoteAddModel } from "../Models/Note/NoteAddModel";
import { NoteType } from "../../enums/NoteType";
import type { DeleteNoteModel } from "../Models/Note/DeleteNoteModel";

const API_BASE_URL = import.meta.env.VITE_ASPNETCORE_API_URL;

export const getUserNotes = async (token: string): Promise<NoteModel[]> => {

    const response = await axios.get<GeneralResultModel<NoteModel[]>>(`${API_BASE_URL}/api/Note/GetUserNotes`, {
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
    });

    const { result, errors, hasErrors } = response.data;

    if (hasErrors || !result) {
        throw new Error(errors?.join(", ") || "Unknown server error");
    }

    return result;
};

export const addNote = async (noteData: NoteAddModel, token: string): Promise<NoteModel> => {
    const form = new FormData();

    if (noteData.kind === NoteType.Text) {
        form.append("Content", noteData.content);
    } else {
        form.append("Image", noteData.file);
    }

    const response = await axios.post<GeneralResultModel<NoteModel>>(
        `${API_BASE_URL}/api/Note/AddNote`,
        form,
        {
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        }
    );

    const { result, errors, hasErrors } = response.data;

    if (hasErrors || !result) {
        throw new Error(errors?.join(", ") || "Unknown server error");
    }

    return result;
};

export const deleteNote = async (payload: DeleteNoteModel, token: string): Promise<string> => {
    const response = await axios.delete<GeneralResultModel<string>>(
        `${API_BASE_URL}/api/Note/DeleteNote`,
        {
            data: payload,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        }
    );

    const { result, errors, hasErrors } = response.data;

    if (hasErrors || !result) {
        throw new Error(errors?.join(", ") || "Failed to delete note");
    }

    return result;
};