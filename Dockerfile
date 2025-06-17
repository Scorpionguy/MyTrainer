# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проекта
COPY *.csproj ./
RUN dotnet restore

# Копируем всё и собираем
COPY . ./
RUN dotnet publish -c Release -o out

# Этап 2: Выполнение
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Указываем порт (если используется)
EXPOSE 80

# Команда для запуска
ENTRYPOINT ["dotnet", "MyTrainer.dll"]