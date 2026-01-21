namespace DynamicForm.Mobile.Services;

public static class ApiConfig
{
    // DEV: Sửa lại IP này cho đúng với máy chạy API
    // Khi chạy trên emulator Android: dùng IP LAN của máy host, ví dụ "http://192.168.1.10:5144"
    // Hoặc nếu API chạy trên localhost: "http://10.0.2.2:5144" (Android emulator)
    // iOS simulator: "http://localhost:5144" hoặc IP LAN
    // Port 5144 là port HTTP mặc định của DynamicForm.API
    public const string BaseUrl = "https://localhost:7220";
}
