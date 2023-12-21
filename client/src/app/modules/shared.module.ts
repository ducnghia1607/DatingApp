import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      timeOut: 2000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }),
    TabsModule.forRoot(),
    NgxSpinnerModule.forRoot({ type: 'line-scale-party' }),
    FileUploadModule,
  ],
  exports: [
    ToastrModule,
    BsDropdownModule,
    TabsModule,
    NgxSpinnerModule,
    FileUploadModule,
  ],
})
export class SharedModule {}
