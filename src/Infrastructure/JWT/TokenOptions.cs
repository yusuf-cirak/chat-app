﻿namespace Infrastructure.JWT;

public record TokenOptions(string Audience,string Issuer, int AccessTokenExpiration,string SecurityKey);