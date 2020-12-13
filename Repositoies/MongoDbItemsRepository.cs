/*
 * 启动docker服务后，项目目录下终端运行：
 * docker ps
 *      罗列docker服务正在运行容器列表
 * docker stop 容器名称
 *      停止正在运行的docker容器
 * docker volume ls
 *      罗列docker容器存储卷标列表
 * docker volume rm 卷标名称
 *      物理级别删除docker容器存储卷标(注意：该操作不可逆！须审慎操作！)
 * docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
 *      创建并以后台模式运行无登录身份校验保护的MongoDB数据库实例docker容器
 *      容器名称为：mongo，映射MongoDB数据库服务端口为：27017与当前主机网络端口27017配对映射
 *      存储卷标为：mongodbdata，保存数据路径为：mongodbdata卷标下的data/db子目录
 * docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongodbadmin -e MONGO_INITDB_ROOT_PASSWORD=mongodb#pass1! mongo
 *      创建并以后台模式运行有身份校验用户名(mongodbadmin)及登录密码(mongodb#pass1!)保护的MongoDB数据库实例docker容器
 *      容器名称为：mongo，映射MongoDB数据库服务端口为：27017与当前主机网络端口27017配对映射
 *      存储卷标为：mongodbdata，保存数据路径为：mongodbdata卷标下的data/db子目录
 *  !!! 注意：！！！
 *      当有身份校验MongoDB实例运行时需要在项目appsettings.json文件中的MongoDBSettings属性内新增User属性，并设置该属性值为：mongodbadmin
 *      并在项目目录下开启终端并运行：
 *      dotnet user-secrets init
 *      该命令执行 dotNet 项目中用户安全密钥功能初始化操作
 *      再执行：
 *      dotnet user-secrets set MongoDBSettings:Password mongodb#pass1!
 *      该命令执行 dotNet 项目内向用户安全密钥内存储MongoDBSettings:Password(即项目应用配置参数MongoDBSettings属性下新建Password属性，该登录密码属性值为：mongodb#pass1!)
 *      在项目Settings目录下的MongoDBSettings.cs文件内新建两个新的公共属性
 *      public string User { get; set; } 和 public string Password { get; set; }
 *      并更新ConnectionString公共属性：
 *      public string ConnectionString { get => $"mongodb://{User}:{Password}@{Host}:{Port}"; }
 * docker network ls
 *      罗列docker服务网络列表
 * docker network create catalog_network
 *      创建名为：catalog_network 的 docker 网络
 * docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongodbadmin -e MONGO_INITDB_ROOT_PASSWORD=mongodb#pass1! --network=catalog_network mongo
 *      创建并以后台模式运行有身份校验用户名(mongodbadmin)及登录密码(mongodb#pass1!)保护的MongoDB数据库实例docker容器
 *      容器名称为：mongo，映射MongoDB数据库服务端口为：27017与当前主机网络端口27017配对映射
 *      存储卷标为：mongodbdata，保存数据路径为：mongodbdata卷标下的data/db子目录
 *      并启用docker网络：catalog_network
 * docker images
 *      罗列docker镜像列表
 * docker run -it --rm -p 8080:80 -e MongoDBSettings:Host=mongo -e MongoDBSettings:Password=mongodb#pass1! --network=catalog_network catalog:v1
 *      以交互模式(该模式允许在终端中以`Ctrl + C`组合键关闭应用镜像)运行名为：catalog 的 REST API 镜像，版本(TAG)是v1版
 *      运行 REST API 镜像应用于 8080 端口
 *      且 MongoDB 数据库主机名称所在的项目应用配置文件(appsettings.json)中的 MongoDBSettings 配置项中的
 *      Host 属性(即 MongoDB 数据库服务器所在主机名称)修改成名为： mongo 运行的 docker (MongoDB数据库)容器
 *      Password 属性(即 MongoDB 数据库服务器用户安全密钥登录密码信息：mongodb#pass1!
 *      并配以 docker 内建网络(--network)为：catalog_network
 * docker login
 *      登录 docker hub，需身份验证(即：docker用户名[cynosure0313]及登录密码)
 * docker tag catalog:v1 cynosure0313/catalog:v1
 *      创建待上传至 docker hub 容器库的 REST API 镜像，版本为：v1
 * docker push cynosure0313/catalog:v1
 *      上传创建成功的名为：cynosure0313/catalog:v1 的 REST API 镜像至 docker hub 容器库
 * docker rmi cynosure0313/catalog:v1
 *      移除本地名为：cynosure0313/catalog:v1 的 REST API 镜像
 * docker rmi catalog:v1
 *      移除本地名为：catalog:v1 的 REST API 镜像
 * docker run -it --rm -p 8080:80 -e MongoDBSettings:Host=mongo -e MongoDBSettings:Password=mongodb#pass1! --network=catalog_network cynosure0313/catalog:v1
 *      以交互模式(该模式允许在终端中以`Ctrl + C`组合键关闭应用镜像)运行名为：cynosure0313/catalog 的 REST API 镜像，版本(TAG)是v1版
 *      目前该版本 REST API 镜像在 docker 服务本地镜像库中没有，所以会自动从 https://hub.docker.com 网络镜像库中自动下载至本地
 *      运行 REST API 镜像应用于 8080 端口
 *      且 MongoDB 数据库主机名称所在的项目应用配置文件(appsettings.json)中的 MongoDBSettings 配置项中的
 *      Host 属性(即 MongoDB 数据库服务器所在主机名称)修改成名为： mongo 运行的 docker (MongoDB数据库)容器
 *      Password 属性(即 MongoDB 数据库服务器用户安全密钥登录密码信息：mongodb#pass1!
 *      并配以 docker 内建网络(--network)为：catalog_network
 * -----------------------------------------------------------------------------------
 * --- 增加REST API 健康检查功能：---
 * 1)、项目目录下开启终端，向项目内新增MongoDB数据库健康检查依赖包：
 *      dotnet add package AspNetCore.HealthChecks.MongoDb
 * 2)、在项目启动类Startup.cs文件内ConfigureServices方法中新增健康检查属性：
 *      services.AddHealthChecks().AddMongoDb(
                mongoDBSettings.ConnectionString,
                name: "mongo",
                timeout: TimeSpan.FromSeconds(3),
                tags: new[] { "ready" }
                );
 * 3)、在项目启动类Startup.cs文件内Configure方法中新增应用健康检查端点路由属性：
 *      endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = (check) => check.Tags.Contains("ready") });
 *      endpoints.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = (_) => false });
 * ---------------------------------------------------------------------------------
 * vscode 下 使用 git hub 进行版本控制操作
 * git config --global user.email "github注册电子邮件(cynosure0313@live.cn)"
 * vscode git 版本控制仓库全局配置 登录用户电子邮件地址
 * git config --global user.name "github注册用户名称(JumperLiu)"
 * vscode git 版本控制仓库全局配置 登录用户名称
 * 登录 github https://github.com 创建新项目仓库(此处项目仓库名为：Catalog-V1)
 * 项目目录下开启终端输入下列命令以执行版本提交：
 * echo "# Catalog-V1" >> README.md
 * git init
 * git add README.md
 * git commit -m "first commit"
 * git branch -M main
 * git remote add origin https://github.com/JumperLiu/Catalog-V1.git
 * git branch -M main
 * git push -u origin main
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositoies
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string DatabaseName = "catalog";
        private const string CollectionName = "items";
        private readonly IMongoCollection<Item> itemsCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            itemsCollection = database.GetCollection<Item>(CollectionName);
        }

        public async Task CreateItemAsync(Item item) => await itemsCollection.InsertOneAsync(item);

        public async Task DeleteItemAsync(Item item) => await itemsCollection.DeleteOneAsync(filterBuilder.Eq(existingItem => existingItem.Id, item.Id));

        public async Task<Item> GetItemAsync(Guid id) => await itemsCollection.Find(filterBuilder.Eq(item => item.Id, id)).SingleOrDefaultAsync();

        public async Task<IEnumerable<Item>> GetItemsAsync() => await itemsCollection.Find(new BsonDocument()).ToListAsync();

        public async Task UpdateItemAsync(Item item) => await itemsCollection.ReplaceOneAsync(filterBuilder.Eq(existingItem => existingItem.Id, item.Id), item);
    }
}