# 1️⃣ مرحلة البناء
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# نسخ ملفات المشروع واستعادة الحزم
COPY *.csproj ./
RUN dotnet restore

# نسخ باقي الملفات وبناء المشروع
COPY . ./
RUN dotnet publish -c Release -o out

# 2️⃣ مرحلة التشغيل
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .

# فتح البورت الافتراضي
EXPOSE 80

# أمر تشغيل التطبيق
ENTRYPOINT ["dotnet", "TrustInsuranceApi1.dll"]
