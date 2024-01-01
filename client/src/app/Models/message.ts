export interface Message {
  id: number;
  senderId: number;
  senderPhotoUrl: string;
  senderUsername: string;
  recipientId: number;
  recipientPhotoUrl: string;
  recipientUsername: string;
  content: string;
  dateSent: Date;
  dateRead?: Date;
}
