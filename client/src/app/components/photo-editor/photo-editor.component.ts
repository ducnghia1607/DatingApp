import { Component, Input } from '@angular/core';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { take } from 'rxjs';
import { User } from 'src/app/Models/User';
import { Member } from 'src/app/Models/member';
import { Photo } from 'src/app/Models/photo';
import { AccountService } from 'src/app/services/account.service';
import { MemberService } from 'src/app/services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent {
  baseUrl = environment.apiUrl;
  user: User | undefined;
  @Input() member: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver: boolean = false;

  constructor(
    private accountService: AccountService,
    private memberService: MemberService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      if (user) {
        this.user = user;
      }
    });

    this.initializeFileUploader();
  }

  initializeFileUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      maxFileSize: 10 * 1024 * 1024,
      autoUpload: false,
      allowedFileType: ['image'],
      removeAfterUpload: true,
    });
    this.hasBaseDropZoneOver = false;

    // Resolve cors
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    // Add new photo to list photo
    this.uploader.onSuccessItem = (item, response, status, header) => {
      if (response) {
        var photo = JSON.parse(response);
        this.member?.photos.push(photo);
      }
    };
  }

  // e : event
  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          // Update new main photo for user
          this.user.photoUrl = photo.url;

          // update the user with new main photo for components subcribe
          this.accountService.setCurrentUser(this.user);

          // update view new main photo in photo-editor component
          this.member.photoUrl = photo.url;
        }

        // Update view status of button main
        this.member?.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id == photo.id) p.isMain = true;
        });
      },
    });
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        if (this.member) {
          // update view in photo-editor -component
          this.member.photos = this.member.photos.filter(
            (x) => x.id != photoId
          );
        }
      },
    });
  }
}
