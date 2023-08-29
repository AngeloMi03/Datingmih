import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guard/auth.guard';
import { TestErrosComponent } from './errors/test-erros/test-erros.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsaveChangeGuard } from './_guard/prevent-unsave-change.guard';
import { MemberDetailsResolvers } from './_resolvers/members-details.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guard/admin.guard';

const routes: Routes = [
  {path : '', component : HomeComponent},
  {
    path : '',
    runGuardsAndResolvers : 'always',
    canActivate : [AuthGuard],
    children : [
      {path : 'members', component : MemberListComponent},
      {path : 'members/:username', component : MemberDetailComponent, resolve : {member : MemberDetailsResolvers}},
      {path : 'member/edit', component : MemberEditComponent, canDeactivate : [PreventUnsaveChangeGuard]},
      {path : 'lists', component : ListsComponent},
      {path : 'messages', component : MessagesComponent},
      {path : 'admin', component : AdminPanelComponent, canActivate : [AdminGuard]},
    ]   
  },
  {path : "errors", component : TestErrosComponent},
  {path : "not-found", component : NotFoundComponent},
  {path : "server-error", component : ServerErrorComponent},
  {path : '**', component : NotFoundComponent, pathMatch : 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
