using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ExportTransportationService
{
    public interface IExportTransportationService
    {
        Entities.ExportTransportation getById(int id);
        List<Entities.ExportTransportation> getAll();
        void insertExportTransportation(Entities.ExportTransportation model);
        void saveExportTransportation(List<int> categoryIds, string flag);
        IPagedList<Entities.ExportTransportation> searchList(SysCustomizedListSearchArg arg, int page, int size, string flag);
        void updateExportTransportation(Entities.ExportTransportation model, string flag);
    }
}
