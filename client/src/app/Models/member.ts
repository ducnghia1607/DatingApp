import { Photo } from './photo';

export interface Member {
  id: number;
  userName: string;
  knownAs: string;
  photoUrl: string;
  age: number;
  city: string;
  country: string;
  created: string;
  lastActive: string;
  lookingFor: string;
  interests: string;
  gender: string;
  introduction: string;
  photos: Photo[];
}
