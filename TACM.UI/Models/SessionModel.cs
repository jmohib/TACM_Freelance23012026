using TACM.Core;
using TACM.Entities;

namespace TACM.UI.Models;

public sealed partial class SessionModel : Model
{
    public SessionModel() : base() { }        

    private string? _subjectID;
    private ushort? _age;
    private string _sex = "";

    public string? SubjectID
    {
        get => _subjectID;
        set
        {
            _subjectID = value;
            OnPropertyChanged(nameof(SubjectID));
        }
    }

    public ushort? Age
    {
        get => _age;
        set
        {
            _age = value;
            OnPropertyChanged(nameof(Age));
        }
    }

    public string Sex
    {
        get => _sex;
        set
        {
            _sex = value;
            OnPropertyChanged(nameof(Sex));
        }
    }

    internal override TEntity? ToEntity<TEntity>() where TEntity : class
    {
        var entity = base.ToEntity<TEntity>() as Session;
        entity!.Sex = _sex.Substring(0,1);

        return entity as TEntity;
    }

    internal override void CopyFromEntity(in object entity)
    {
        base.CopyFromEntity(entity);

        if (entity is not Session session)
            return;

        Sex = AppConstants.SEX_DICTIONARIES[session.Sex];
    }
}
