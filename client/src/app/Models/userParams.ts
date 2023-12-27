import { User } from './User';

// Lí do không tạo interface mà tạo class vì tạo class có thể khởi tạo các giá trị trước cho nó
export class UserParams {
  pageNumber: number = 1;
  pageSize: number = 5;
  gender: string;
  minAge: number = 18;
  maxAge: number = 99;
  orderBy: string = 'lastActive';
  constructor(user: User) {
    this.gender = user.gender == 'male' ? 'female' : 'male';
  }
}
