﻿using MyRow = Serene.Administration.LanguageRow;
using MyRequest = Serenity.Services.SaveRequest<Serene.Administration.LanguageRow>;
using MyResponse = Serenity.Services.SaveResponse;


namespace Serene.Administration;

public interface ILanguageSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }
public class LanguageSaveHandler : SaveRequestHandler<MyRow, MyRequest, MyResponse>, ILanguageSaveHandler
{
    public LanguageSaveHandler(IRequestContext context)
         : base(context)
    {
    }
}