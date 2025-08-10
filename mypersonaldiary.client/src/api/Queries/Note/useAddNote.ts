import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { NoteModel } from "../../Models/Note/NoteModel";
import type { NoteAddModel } from "../../Models/Note/NoteAddModel";
import { addNote } from "../../Services/NoteService";

export const useAddNote = (token: string) => {
    const queryClient = useQueryClient();

    return useMutation<NoteModel, Error, NoteAddModel>({
        mutationFn: (noteData) => addNote(noteData, token),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["userNotes", token] });
        },
    });
};