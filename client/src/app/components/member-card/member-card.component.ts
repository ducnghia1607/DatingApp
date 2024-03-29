import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/Models/member';
import { MemberService } from 'src/app/services/member.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined;
  constructor(
    private memberService: MemberService,
    private toast: ToastrService,
    public presenceService: PresenceService
  ) {}
  ngOnInit(): void {

  }

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => {
        next: () => this.toast.success('You have liked ' + member.knownAs);
      },
    });
  }
}
