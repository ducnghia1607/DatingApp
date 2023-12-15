import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      timeOut: 2000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }), // ToastrModule added
  ],
  exports: [ToastrModule, BsDropdownModule],
})
export class SharedModule {}
