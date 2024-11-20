using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Controllers
{
    public class LpLocalizationControllerBase : LpApiControllerBase
    {
        #region Internals & ctor
        private readonly string i18nDir;
        private readonly ILpLocalizationService localizationService;
        private readonly IFsService fsService;
        private readonly IJsonService jsonService;

        public LpLocalizationControllerBase(
            string i18nDir,
            ILpLocalizationService localizationService,
            IFsService fsService,
            IJsonService jsonService)
        {
            this.i18nDir = i18nDir;
            this.localizationService = localizationService;
            this.fsService = fsService;
            this.jsonService = jsonService;
        }
        #endregion

        #region Protected interface
        protected List<LpLocaleInfo> GetAvailableLocales(List<string> sources)
        {
            return localizationService.GetAvailableLocales(sources);
        }

        /// <summary>
        /// Returns compact version of i18next JS lib translation file
        /// </summary>
        /// <param name="lng">{{lng}} param of the i18next lib</param>
        /// <param name="ns">{{ns}} param of the i18next lib</param>
        protected async Task<string> BuildTranslationFileContentAsync(
            string lng,
            string ns = "translation",
            CancellationToken cancellationToken = default)
        {
            var filePath = fsService.PathCombine(
                i18nDir,
                LpLocalizationDocument.GetFileName(ns, lng));
            var doc = await localizationService.LoadDocumentAsync(filePath, cancellationToken);
            var trDict = doc.Items.ToDictionary(GetTranslationFileKey, GetTranslationFileValue);
            return jsonService.SerializeObject(trDict);
        }
        #endregion

        #region Internals
        private static string GetTranslationFileKey(
            AbstractLpLocalizationResourceDocumentEntry e)
        {
            return e switch
            {
                LpStringLocalizationResourceDocumentEntry s => $"s.{s.Key}",
                LpFileLocalizationResourceDocumentEntry f => $"f.{f.Key}",
                _ => throw new NotSupportedException(
                    "Wrong AbstractLpLocalizationResourceDocumentEntry")
            };
        }

        private static string GetTranslationFileValue(
            AbstractLpLocalizationResourceDocumentEntry e)
        {
            return e switch
            {
                LpStringLocalizationResourceDocumentEntry s => s.Value,
                LpFileLocalizationResourceDocumentEntry f => f.Path,
                _ => throw new NotSupportedException(
                    "Wrong AbstractLpLocalizationResourceDocumentEntry")
            };
        }
        #endregion
    }
}
