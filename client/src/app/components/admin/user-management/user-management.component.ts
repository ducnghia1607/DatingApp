import { Component } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { AdminService } from 'src/app/services/admin.service';
import { RoleModalComponent } from '../role-modal/role-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent {
  bsModalRef: BsModalRef<RoleModalComponent> =
    new BsModalRef<RoleModalComponent>();
  users: any[] = [];
  availableRoles: string[] = ['Admin', 'Moderator', 'Member'];
  selectedRoles: string[] = [];

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe((u: any) => {
      this.users = u;
    });
  }

  openModalWithComponent(user: any) {
    console.log(user);
    const config = {
      class: 'modal-dialog modal-dialog-centered',
      initialState: {
        availableRoles: this.availableRoles,
        // selectedRoles: ...user.roles,
        // Copy mảng để check với user.roles
        selectedRoles: [...user.roles],
        username: user.userName,
      },
    };

    this.bsModalRef = this.modalService.show(RoleModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;

        if (!this.arrayEqual(selectedRoles!, user.roles)) {
          console.log(user.userName);
          this.adminService.editRole(user.userName, selectedRoles!).subscribe({
            next: (roles: any) => {
              user.roles = roles;
            },
          });
        }
      },
    });
  }

  private arrayEqual(arr1: any, arr2: any) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }
}
