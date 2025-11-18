==Tạo key ==
# Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Max 256 })) -replace '[+/=]','' | Select-Object -First 32

# Trong thư mục project
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "key vừa tạo"

"Jwt": {
  "Key": "DEV-USE-USER-SECRETS",
  "Issuer": "",
  "Audience": ""
}
==>Lưu ý:
# 1. Vào đúng thư mục project
cd "D:\TaiLieu\Nam3\HocKy1\LapTrinhWebb\BTL\BTL.WebApi"

# 2. Khởi tạo User Secrets (chỉ chạy 1 lần)
dotnet user-secrets init

# 3. Tạo key JWT ngẫu nhiên và lưu
dotnet user-secrets set "Jwt:Key" "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6"

# 4. Kiểm tra
dotnet user-secrets list