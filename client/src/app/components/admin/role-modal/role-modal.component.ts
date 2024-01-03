import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-role-modal',
  templateUrl: './role-modal.component.html',
  styleUrls: ['./role-modal.component.css'],
})
export class RoleModalComponent {
  closeBtnName = '';
  availableRoles: any[] = [];
  selectedRoles: any[] = [];
  username = '';
  constructor(public bsModalRef: BsModalRef) {}

  updateChecked(role: string) {
    const index = this.selectedRoles.indexOf(role);
    index !== -1
      ? this.selectedRoles.splice(index, 1)
      : this.selectedRoles.push(role);
  }
}
