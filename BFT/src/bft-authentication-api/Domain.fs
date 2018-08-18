namespace BFT.AuthenticationApi

[<AutoOpen>]
module ExceptionExtensions =

    type System.Exception with

        member this.GetDetails () =
            match isNull this.InnerException with
            | true -> this.Message
            | false ->
                let message = this.InnerException.GetDetails()
                match this.Message = message with
                | true -> this.Message
                | false -> sprintf "%s%s%s" this.Message System.Environment.NewLine message

module Models =

    type DatabaseId = private DatabaseId of string

    type EndpointUrl = private EndpointUrl of string

    type AccountKey = private AccountKey of string

    type Email = private Email of string

    type Password = private Password of string

    type Token = private Token of string

    type ExpirationDate = private ExpirationDate of System.DateTimeOffset

    type TokenRequestError =
        | RequiredEmail
        | InvalidEmail of string
        | RequiredPassword
        | PasswordLongerThan of int

    type TokenRequest = {
        Email : Email
        Password : Password
    }

    type TokenResult = {
        IssuedTo : Email
        Token : Token
        ExpiresOn : ExpirationDate
    }

    type GenerateToken = TokenRequest -> Result<TokenResult, TokenRequestError>

    [<RequireQualifiedAccess>]
    module Strings =

        let isNullOrEmpty = System.String.IsNullOrEmpty

        let toUpper (str : string) = str.ToUpper()

        let toLower (str : string) = str.ToLower()

        let private createWithContinuation ok error value =
            match isNullOrEmpty value with
            | true -> error value
            | false -> ok value

        let create customType errorMessage =
            let ok (value : string) =
                value.Trim()
                |> customType
                |> Result.Ok

            let error _ = Result.Error errorMessage

            createWithContinuation ok error

        let createOptional customType =
            let ok (value : string) =
                value.Trim()
                |> customType
                |> Some

            let error _ = None

            createWithContinuation ok error

    module Email =

        let apply f (Email x) = f x

        let value x = apply id x

        let create =
            let validateFormat str =
                let email = value str
                if email.Contains("@") then
                    Strings.toLower email |> Email |> Result.Ok
                else Result.Error (TokenRequestError.InvalidEmail "Email must contain an '@' sign")

            Strings.create Email TokenRequestError.RequiredEmail
            >> Result.bind validateFormat

        let equals (Email x) (Email y) =
            x = y

    module Password =
        open System
        open System.Security.Cryptography
        open System.Text

        let apply f (Password x) = f x

        let value x = apply id x

        let create =
            let hash str =
                let postSalt = "_buffer_9#00!#8423-12834)*@$920*"
                let preSalt = "bft_salt_"
                let data = Encoding.UTF8.GetBytes(sprintf "%s%s%s" preSalt (value str) postSalt)
                use provider = new SHA256CryptoServiceProvider()

                provider.ComputeHash(data)
                |> Convert.ToBase64String
                |> Password

            let validateLength str =
                if String.length (value str) > 15 then
                    TokenRequestError.PasswordLongerThan 15 |> Result.Error
                else Result.Ok str

            Strings.create Password TokenRequestError.RequiredPassword
            >> Result.bind validateLength >> Result.map hash

        let equals (Password x) (Password y) =
            x = y

    module Token =

        let apply f (Token x) = f x

        let value x = apply id x

        let createOptional = Strings.createOptional Token
        
[<RequireQualifiedAccess>]
module Dto =

    [<CLIMutable>]
    type MessageResult = {
        Message : string
    }

    [<CLIMutable>]
    type TokenRequest = {
        Email : string
        Password : string
    }

    [<CLIMutable>]
    type TokenResult = {
        IssuedTo : string
        Token : string
        ExpiresOn : System.DateTimeOffset
    }