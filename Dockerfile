# Chỉ định image nền tảng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Chỉ định thư mục làm việc
WORKDIR /app

# Sao chép file csproj và khôi phục phụ thuộc
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ mã nguồn vào container
COPY . ./

# Build ứng dụng
RUN dotnet publish -c Release -o /out

# Chỉ định image nền tảng .NET Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Chỉ định thư mục làm việc
WORKDIR /app

# Sao chép ứng dụng đã build từ container build vào container runtime
COPY --from=build /out .

# Mở cổng cho ứng dụng
EXPOSE 80

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "MyAPI.dll"]
