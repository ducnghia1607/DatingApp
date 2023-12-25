namespace API.DTOs;

public class UserDto
{
    // Doi tuong tra ve khi dang nhap va dang ky , tra ve object nay cho client , tuong uoc voi User Entity ben Angular 
    public string Username { get; set; }
    public string Token { get; set; }

    public string PhotoUrl { get; set; }

    public string KnownAs { get; set; }
}
