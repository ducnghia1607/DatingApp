import { ResolveFn } from '@angular/router';
import { MemberService } from '../services/member.service';
import { Member } from '../Models/member';
import { inject } from '@angular/core';

export const memberDetailResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MemberService);

  return memberService.getMember(route.paramMap.get('username')!);
};
