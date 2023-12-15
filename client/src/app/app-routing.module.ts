import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { MembersComponent } from './components/members/members.component';
import { MemberDetailComponent } from './components/member-detail/member-detail.component';
import { MessagesComponent } from './components/messages/messages.component';
import { ListsComponent } from './components/lists/lists.component';
import { authGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MembersComponent },
      { path: 'members/:id', component: MemberDetailComponent },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent },
    ],
  },
  { path: '**', component: HomeComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
