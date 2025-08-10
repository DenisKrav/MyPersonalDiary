import { useMutation } from "@tanstack/react-query";
import type { DeleteNoteModel } from "../../Models/Note/DeleteNoteModel";
import { deleteNote } from "../../Services/NoteService";

export const useDeleteNote = (token: string) => {
  return useMutation<string, Error, DeleteNoteModel>({
    mutationFn: (payload) => deleteNote(payload, token),
  });
};