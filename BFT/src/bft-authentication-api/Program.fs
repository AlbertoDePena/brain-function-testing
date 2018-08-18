module BFT.AuthenticationApi.Program

open System
open System.IO
open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.IdentityModel.Tokens
open Giraffe
open System.Text
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Cors.Infrastructure

let authorize =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    let dto : Dto.MessageResult = { Message = ex.GetDetails() }
    clearResponse >=> ServerErrors.internalError (json dto)

let handleTokenRequest : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let validatePassword  (dto : Dto.TokenRequest) =
                if String.IsNullOrEmpty dto.Password then
                    Result.Error "Password is required"
                else Result.Ok dto

            let validateEmail  (dto : Dto.TokenRequest) =
                if String.IsNullOrEmpty dto.Email then
                    Result.Error "Email is required"
                else Result.Ok dto

            let! dto = ctx.BindJsonAsync<Dto.TokenRequest>()

            return! Successful.OK dto next ctx
        }

let handleInvalidRoute : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let dto : Dto.MessageResult = { Message = "The resource you requested cannot be found" }
        RequestErrors.notFound (json dto) next ctx

let webApp =
    choose [
        GET >=>
            choose [
                route "/api/token" >=> authorize >=> text "Not implemented"
            ]
        POST >=>
            choose [
                route "/api/token" >=> handleTokenRequest
            ]
        handleInvalidRoute
    ]

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let configureApp (app : IApplicationBuilder) =
    app.UseCors(configureCors)
       .UseAuthentication()
       .UseGiraffeErrorHandler(errorHandler)
       .UseStaticFiles()
       .UseGiraffe webApp

let authenticationOptions (opts : AuthenticationOptions) =
    opts.DefaultAuthenticateScheme <- JwtBearerDefaults.AuthenticationScheme
    opts.DefaultChallengeScheme <- JwtBearerDefaults.AuthenticationScheme

let jwtBearerOptions (config : IConfiguration) (opts : JwtBearerOptions) =
    let jwtConfig = config.GetSection("Jwt")
    let signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.["SecretKey"]))
                        
    opts.TokenValidationParameters <- TokenValidationParameters (
        ValidIssuer = jwtConfig.["Issuer"],
        ValidAudience = jwtConfig.["Audience"],
        IssuerSigningKey = signinKey,
        ClockSkew = TimeSpan.Zero
    )

let configureServices (services : IServiceCollection) =
    let config =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json")
            .Build()

    services
        .AddCors()
        .AddGiraffe()
        .AddAuthentication(authenticationOptions)
        .AddJwtBearer(fun opts -> jwtBearerOptions config opts) |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (logLevel : LogLevel) = logLevel.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "wwwroot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0
