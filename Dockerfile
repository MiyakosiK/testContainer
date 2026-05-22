# デバッグ コンテナーをカスタマイズする方法と、Visual Studio がこの Dockerfile を使用してより高速なデバッグのためにイメージをビルドする方法については、https://aka.ms/customizecontainer をご覧ください。

# このステージは、VS から高速モードで実行するときに使用されます (デバッグ構成の既定値)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# このステージは、サービス プロジェクトのビルドに使用されます
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 引数の取得
ARG OWNER
ARG YOUR_GITHUB_PAT

#Node
RUN apt-get update && \
    apt-get install -y sudo && \
    curl -sL https://deb.nodesource.com/setup_20.x | sudo -E bash - && \
    apt-get install -y nodejs

# 取得した引数より、Githubパッケージをインストールための設定を追加
RUN dotnet nuget add source --username $OWNER --password $YOUR_GITHUB_PAT --store-password-in-clear-text --name github "https://nuget.pkg.github.com/KDW-DevDiv/index.json"

# csprojのみコピー（キャッシュ活用）
COPY ["BizKotei/BizKotei/BizKotei.csproj", "BizKotei/BizKotei/"]
COPY ["BizKotei/BizKotei.Client/BizKotei.Client.csproj", "BizKotei/BizKotei.Client/"]
RUN dotnet restore "BizKotei/BizKotei/BizKotei.csproj"

#NPM
COPY ["BizKotei/BizKotei/package.json", "BizKotei/BizKotei/"]
WORKDIR "/src/BizKotei/BizKotei"
RUN npm install

#build
COPY . /src
WORKDIR "/src/BizKotei/BizKotei"
RUN dotnet build "./BizKotei.csproj" -c $BUILD_CONFIGURATION -o /app/build

#publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/BizKotei/BizKotei"
RUN dotnet publish "./BizKotei.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV TZ=Asia/Tokyo
ENV ASPNETCORE_ENVIRONMENT=Staging
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BizKotei.dll"]