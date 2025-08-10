export interface NoteModel {
        id: string;
        userId: string;
        content: string | null;
        imageContentType: string | null;
        imageSize: number | null;
        imageData: string | null; 
        createdAt: string;
}