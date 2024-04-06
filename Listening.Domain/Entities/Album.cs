
using Common.Domain.Models;

namespace Listening.Domain.Entities
{
    
    public record Album : AggregateRootEntity, IAggregateRoot
    {
        private Album() { }

        /// <summary>
        /// 用户是否可见（完善后才显示，或者已经显示了，但是发现内部有问题，就先隐藏，调整了再发布）
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// 标题
        /// </summary>
        public MultilingualString Name { get; private set; }

        /// <summary>
        /// 列表中的显示序号
        /// </summary>
        public int SequenceNumber { get; private set; }

        public Guid CategoryId { get; private set; }

        public static Album Create(Guid id, int sequenceNumber, MultilingualString name, Guid categoryId)
        {
            Album album = new Album();
            album.Id = id;
            album.SequenceNumber = sequenceNumber;
            album.Name = name;
            album.CategoryId = categoryId;
            album.IsVisible = false;//Album新建以后默认不可见，需要手动Show
            return album;
        }
        public Album ChangeSequenceNumber(int value)
        {
            this.SequenceNumber = value;
            return this;
        }

        public Album ChangeName(MultilingualString value)
        {
            this.Name = value;
            return this;
        }
        public Album Hide()
        {
            this.IsVisible = false;
            return this;
        }
        public Album Show()
        {
            this.IsVisible = true;
            return this;
        }
    }
}
