using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private LiftContext _context;
        private ITrainerRepository? _trainerRepository;
        private IProductRepository? _productRepository;
        private IFitnessClassRepository? _fitnessClassRepository;
        private IMembershipRepository? _membershipRepository;
        private IContactMessageRepository? _contactMessageRepository;
        private INutritionRepository? _nutrition;
        private IMemberRepository? _memberRepository;
        private ICartRepository? _cartRepository;
        private IChatRepository? _chatRepository;
        private IClassReservationRepository? _classReservationRepository;

        private IReviewRepository? _reviewRepository;

        public RepositoryWrapper(LiftContext context)
        {
            _context = context;
        }

        public ITrainerRepository Trainer
        {
            get
            {
                if (_trainerRepository == null)
                {
                    _trainerRepository = new TrainerRepository(_context);
                }
                return _trainerRepository;
            }
        }

        public IProductRepository Product
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            }
        }

        public IFitnessClassRepository FitnessClass
        {
            get
            {
                if (_fitnessClassRepository == null)
                {
                    _fitnessClassRepository = new FitnessClassRepository(_context);
                }
                return _fitnessClassRepository;
            }
        }

        public IMembershipRepository Membership
        {
            get
            {
                if (_membershipRepository == null)
                {
                    _membershipRepository = new MembershipRepository(_context);
                }
                return _membershipRepository;
            }
        }

        public IContactMessageRepository ContactMessage
        {
            get
            {
                if (_contactMessageRepository == null)
                {
                    _contactMessageRepository = new ContactMessageRepository(_context);
                }
                return _contactMessageRepository;
            }
        }

        public INutritionRepository Nutrition
        {
            get
            {
                if (_nutrition == null)
                {
                    _nutrition = new NutritionRepository(_context);
                }
                return _nutrition;
            }
        }

        public IMemberRepository Member
        {
            get
            {
                if (_memberRepository == null)
                {
                    _memberRepository = new MemberRepository(_context);
                }
                return _memberRepository;
            }
        }

        public ICartRepository Cart
        {
            get
            {
                if (_cartRepository == null)
                {
                    _cartRepository = new CartRepository(_context);
                }
                return _cartRepository;
            }
        }

        public IChatRepository Chat
        {
            get
            {
                if (_chatRepository == null)
                {
                    _chatRepository = new ChatRepository(_context);
                }
                return _chatRepository;
            }
        }

        public IClassReservationRepository ClassReservation
        {
            get
            {
                if (_classReservationRepository == null)
                {
                    _classReservationRepository = new ClassReservationRepository(_context);
                }
                return _classReservationRepository;
            }
        }

        public IReviewRepository Review
        {
            get
            {
                if (_reviewRepository == null)
                {
                    _reviewRepository = new ReviewRepository(_context);
                }
                return _reviewRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}