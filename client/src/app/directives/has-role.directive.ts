import {
  Directive,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AccountService } from '../services/account.service';
import { User } from '../Models/User';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]',
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];

  user: User = {} as User;
  constructor(
    private viewContainerRef: ViewContainerRef, // Represents a container where one or more views can be attached to a component.
    private templateRef: TemplateRef<any>, // Represents an embedded template that can be used to instantiate embedded views. To instantiate embedded views based on a template, use the ViewContainerRef method createEmbeddedView().
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        }
      },
    });
  }
  ngOnInit(): void {
    if (this.user.roles.some((r) => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
