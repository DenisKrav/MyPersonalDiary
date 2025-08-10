export type NoteAddModel =
  | { kind: "Text"; content: string } 
  | { kind: "Image"; file: File };  