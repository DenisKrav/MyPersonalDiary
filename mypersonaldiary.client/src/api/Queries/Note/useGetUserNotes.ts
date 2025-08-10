import { useQuery } from "@tanstack/react-query";
import type { NoteModel } from "../../Models/Note/NoteModel";
import { getUserNotes } from "../../Services/NoteService";

export const useGetUserNotes = (token: string) => {
    return useQuery<NoteModel[], Error>({
        queryKey: ["userNotes", token],
        queryFn: () => getUserNotes(token),
    });
};