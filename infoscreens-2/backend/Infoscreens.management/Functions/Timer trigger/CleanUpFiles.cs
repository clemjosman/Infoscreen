using Infoscreens.Common.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.file.Interfaces;
using vesact.common.Log;

namespace Infoscreens.Management.Functions.Timer_trigger
{
    public class CleanUpFiles
    {
        #region Constructor / Dependency Injection

        //private readonly ILogger<CleanUpFiles> _logger;
        //private readonly IDatabaseRepository _databaseRepository;
        //private readonly IFileHelper _fileHelper;

        public CleanUpFiles(
        //    ILogger<CleanUpFiles> logger,
        //    IDatabaseRepository databaseRepository,
        //    IFileHelper fileHelper
        )
        {
        //    _logger = logger;
        //    _databaseRepository = databaseRepository;
        //    _fileHelper = fileHelper;
        }

        #endregion Constructor / Dependency Injection

        //const string FUNCTION_NAME = "CleanUpFiles";
        //[Disable] // Disabled as not tested enough, to avoid issues with deleted files and as there isn't too much data, this was kept as is
        //[Function(FUNCTION_NAME)]
        //public async Task RunAsync([TimerTrigger("0 0 0 * * *", RunOnStartup = false)]TimerInfo _)
        //{
        //    try
        //    {
        //        _logger.LogDebug(new LogItem(10, $"Timer Function {FUNCTION_NAME}() called."));
        //
        //        // Gathering all referenced file ids
        //        var referencedFileIds = new List<int>();
        //        referencedFileIds.AddRange((await _databaseRepository.GetAllNewsAsync()).Where(n => n.FileId.HasValue).Select(n => n.FileId.Value).ToList());
        //
        //        // Calling file service for cleanup
        //        _fileHelper.CleanUp(referencedFileIds);
        //
        //        _logger.LogDebug(new LogItem(11, $"Timer Function {FUNCTION_NAME}() finished."));
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
        //    }
        //}
    }
}
