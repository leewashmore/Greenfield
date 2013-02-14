using System;

namespace Aims.Core
{
	/// <summary>
	/// There are 2 types of portfolios:
	/// - Broad-global-active (BGR) portfolios which are regular ones and
	/// - Bottom-up (BU) portfolios which are portfolios and securities at the same times.
	/// Being securities makes these BU-portofolios being owned by other BU-portfolios and/or (???) BGA portfolios.
	/// This class represents both of them.
    /// In order to figure out which portfolio is a BU portfolio you need to check the LOOK_THRU_FUND property of a security and find that portfolio there.
	/// </summary>
    public interface IPortfolio
    {
        String Id { get; }
		String Name { get; }
    }
}
