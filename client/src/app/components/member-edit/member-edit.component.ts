import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from 'src/app/Models/User';
import { Member } from 'src/app/Models/member';
import { AccountService } from 'src/app/services/account.service';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
      // $event.prevent();
    }
  }
  member: Member | undefined;
  user: User | undefined | null = null;
  // editForm:NgForm | undefined;

  // @HostListener('window:beforeunload', ['$event'])
  // onBeforeUnload($event: any) {
  //   if (this.editForm?.dirty) {
  //     $event.returnValue = true;
  //   }
  // }

  constructor(
    private accountService: AccountService,
    private memberService: MemberService,
    private toast: ToastrService
  ) {
    accountService.currentUser$.pipe(take(1)).subscribe({
      next: (res) => {
        if (res) {
          this.user = res;
        }
      },
    });
  }
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if (!this.user) return;
    this.memberService.getMember(this.user.username).subscribe((res) => {
      this.member = res;
    });
  }

  // updateMember(editForm: NgForm) {
  //   console.log(editForm.value);
  //   this.toast.success('Updated profile successfully');
  //   editForm.reset(this.member);
  // }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: (_) => {
        this.toast.success('Updated profile successfully');
        this.editForm?.reset(this.member);
      },
    });
  }
}
