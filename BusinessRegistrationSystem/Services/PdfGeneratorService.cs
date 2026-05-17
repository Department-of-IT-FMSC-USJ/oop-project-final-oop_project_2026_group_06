using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BusinessRegistrationSystem.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BusinessRegistrationSystem.Services
{
    public class PdfGeneratorService
    {
        public PdfGeneratorService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateArticlesOfAssociation(BusinessRegistration model)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.TimesNewRoman));
                    page.Content().Element(x => ComposeContent(x, model));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeContent(IContainer container, BusinessRegistration model)
        {
            // Build subscribers list
            var subscribers = new List<(string Name, string NicOrBr, string Address, int Shares)>();
            int totalShares = 0;

            if (model.Directors != null)
                foreach (var d in model.Directors.Where(x => x.IsShareholder))
                {
                    int s = d.NumberOfShares ?? 0;
                    subscribers.Add(($"{d.Title} {d.FirstNames} {d.Surname}".Trim(), d.NIC, d.Address, s));
                    totalShares += s;
                }

            if (model.Shareholders != null)
                foreach (var sh in model.Shareholders)
                {
                    subscribers.Add(($"{sh.Title} {sh.FirstNames} {sh.Surname}".Trim(), sh.NIC, sh.Address, sh.NumberOfShares));
                    totalShares += sh.NumberOfShares;
                }

            string companyName = (model.ReservationName ?? "THE COMPANY").ToUpper();
            string today = DateTime.Now.ToString("dd MMMM yyyy").ToUpper();

            container.Column(col =>
            {
                col.Spacing(8);

                // ── TITLE ──────────────────────────────────────────────────────
                col.Item().AlignCenter().Text("Articles of Association").FontSize(16).Bold();
                col.Item().AlignCenter().Text("Of").FontSize(14);
                col.Item().AlignCenter().Text(companyName).FontSize(14).Bold();
                col.Item().LineHorizontal(1).LineColor(Colors.Black);

                // ── PREAMBLE ───────────────────────────────────────────────────
                col.Item().PaddingTop(5).Text("The Model Article contained in the first schedule to the Companies Act No.07 of 2007 shall not apply to the Association and the following rules be deemed to be incorporated herewith as the Articles of Association of the Association subject to repeal, change, or modification by a special resolution. In the construction of these Articles words importing the masculine gender only shall include the feminine gender and vice versa and word importing the singular number only shall include the plural number and vice versa.");

                // ── PRIMARY OBJECTS ────────────────────────────────────────────
                col.Item().PaddingTop(4).Text("Primary Objects").Bold().FontSize(11);

                if (!string.IsNullOrEmpty(model.Objectives))
                {
                    var objectives = model.Objectives.Split(new[] { '\n', '.', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    int objNum = 1;
                    foreach (var obj in objectives)
                    {
                        var trimmed = obj.Trim();
                        if (trimmed.Length > 3)
                        {
                            col.Item().PaddingLeft(10).Text($"{objNum}. {trimmed}.");
                            objNum++;
                        }
                    }
                }

                // ── NATURE OF BUSINESS ─────────────────────────────────────────
                col.Item().PaddingTop(4).Text("Nature of Business").Bold().FontSize(11);
                col.Item().Text(!string.IsNullOrEmpty(model.NatureOfBusiness)
                    ? model.NatureOfBusiness
                    : "To be described.");

                // ── SECTION A: SHARES ──────────────────────────────────────────
                col.Item().PaddingTop(8).Text("A. SHARES").Bold().FontSize(12);

                // Shareholding Structure Table
                col.Item().Text("SHAREHOLDING STRUCTURE").Bold().FontSize(10).Underline();
                col.Item().PaddingTop(3).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(2);
                        c.RelativeColumn(4);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Name").Bold().FontSize(9);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("NIC / BR").Bold().FontSize(9);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Address").Bold().FontSize(9);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Number of Shares").Bold().FontSize(9);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Percentage").Bold().FontSize(9);
                    });

                    foreach (var sub in subscribers)
                    {
                        double pct = totalShares > 0 ? Math.Round((double)sub.Shares / totalShares * 100, 2) : 0;
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(sub.Name).FontSize(9);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(sub.NicOrBr).FontSize(9);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(sub.Address).FontSize(9);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(sub.Shares.ToString("N0")).FontSize(9);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{pct}%").FontSize(9);
                    }
                });

                col.Item().PaddingTop(3).Text($"Total Issued Shares: {totalShares:N0} Ordinary Shares").Bold().FontSize(9);

                // Articles 1–4
                Article(col, "1. Issue of shares");
                Sub(col, "(1)", "Subject to articles 1 (2) and 1 (3), of these articles, the board may issue such shares to such persons as it thinks fit in accordance with section 51 of this Act. Where the shares confer rights other than those specified in subsection (2) of section 49 of this Act, or impose any obligation on the holder, the board must approve terms of issue which set out the rights and obligations attached to the shares as required by subsection (2) of section 51.");
                Sub(col, "(2)", "Before it issues shares, the board must decide the consideration for which the shares will be issued. The consideration must be fair and reasonable to the company and to all existing shareholders.");
                Sub(col, "(3)", "Where the company issue shares which rank equally with or prior to existing shares, those shares must be offered to the holders of the existing shares in a manner which would, if accepted, maintain the relative voting and distribution rights of those shareholders. The offer must remain open for acceptance for a reasonable time.");

                Article(col, "2. Calls on shares");
                Sub(col, "(1)(a)", "On a fixed date, the holder must pay that amount on that date;");
                Sub(col, "(1)(b)", "When called on to do so by the board, the board may at any time give written notice to the holder requiring the payment to be made within a specified period of not less than twenty working days, and the payment must be made in accordance with that notice. Any amount not paid by the due date shall carry interest at a rate fixed by the board not exceeding ten per cent per annum, accruing daily. The board may waive payment of interest.");
                Sub(col, "(2)", "Joint holders of a share are jointly and severally liable for any payments to be made under paragraph (1) of this article.");
                Sub(col, "(3)", "The company has a lien on every share to which paragraph (a) of article 1 applies, and on every distribution payable in respect of that share, for all amounts presently due and payable to the company in respect of that share.");
                Sub(col, "(4)", "The company may sell in such manner as the board thinks fit, any shares on which the company has a lien, if the company has given written notice and the shareholder has failed to make the payment within ten working days. The transfer may be signed on behalf of the purchaser by any person appointed to do so by the board, and the purchaser shall be registered as the holder of the shares transferred and his title shall not be affected by any irregularity or invalidity in the sale.");
                Sub(col, "(5)", "The proceeds of a sale under paragraph (4) shall be received by the company and applied first in payment of the costs of sale, and then in payment of the amount in respect of which the lien arose. The remainder shall be paid to the person entitled to the shares, at the time of the sale.");

                Article(col, "3. Distributions");
                Sub(col, "(1)", "The company may make distributions to shareholders in accordance with section 56 of this Act. Subject to paragraph (2) of this article, every dividend must be approved by the board and by an ordinary resolution of the shareholders. The board must be satisfied that the company will immediately after the distribution, satisfy the solvency test. The directors who vote in favor of the distribution must sign a certificate of their opinion to that effect.");
                Sub(col, "(2)", "The board may from time to time approve the payment of an interim dividend to shareholders, where that appears to be justified by the company's profits, without the need for approval by an ordinary resolution of the shareholders. The board must be satisfied that the company will immediately after the interim dividend is paid, satisfy the solvency test.");
                Sub(col, "(3)", "The company is deemed to have satisfied the solvency test if — (a) it is able to pay its debts as they fall due in the normal course of business; and (b) the value of its assets is greater than the sum of the value of its liabilities and its stated capital.");

                Article(col, "4. Share register, share certificates and transfer and transmission of shares");
                Sub(col, "(1)", "The company must maintain a share register, which complies with section 123 of this Act. The share register must be kept at the registered office of the company or at any other place in Sri Lanka, notice of which has been given to the Registrar.");
                Sub(col, "(2)", "Where shares are to be transferred, a form of transfer signed by the holder or by his legal representative shall be delivered to the company. The transfer must be signed by the transferee if the share imposes any liability on its holder.");
                Sub(col, "(3)", "The board may resolve to refuse to register a transfer of a share within six weeks of receipt of the transfer, if any amount payable to the company in respect of the share is due but unpaid.");
                Sub(col, "(4)", "Where a joint holder of a share dies, the remaining holders shall be treated by the company as the holders of that share. Where the sole holder of a share dies, that shareholder's legal representative shall be the only person recognized by the company as having any title to or interest in the share.");
                Sub(col, "(5)", "Any person who becomes entitled to a share as a consequence of the death, bankruptcy or insolvency or incapacity of a shareholder may be registered as the holder of that shareholder's shares upon making a request in writing to the company to be so registered.");
                Sub(col, "(6)", "Where the company issues shares or the transfer of any shares is entered on the share register, the company must within two months complete and have ready for delivery a share certificate in respect of the shares.");

                // ── SECTION B: MEETINGS ────────────────────────────────────────
                col.Item().PaddingTop(8).Text("B. MEETINGS OF SHAREHOLDERS").Bold().FontSize(12);

                Article(col, "5. Rules relating to meetings of shareholders");
                col.Item().PaddingLeft(10).Text("A meeting of shareholders may determine its own procedure, to the extent that it is not governed by these articles.");

                Article(col, "6. Notice of meetings");
                Sub(col, "(1)(a)", "Not less than fifteen working days before the meeting, if the company is not a private company and it is intended to propose a resolution as a special resolution at the meeting;");
                Sub(col, "(1)(b)", "Not less than ten working days before the meeting, in any other case.");
                Sub(col, "(2)", "The notice must set out — (a) The nature of the business to be transacted at the meeting in sufficient detail to enable a shareholder to form a reasoned judgment in relation to it; and (b) The text of any resolution to be submitted to the meeting.");
                Sub(col, "(3)", "An irregularity in a notice of a meeting is waived if all the shareholders entitled to attend and vote at the meeting attend the meeting without protest as to the irregularity, or if all such shareholders agree to the waiver.");
                Sub(col, "(4)", "If a meeting of shareholders is adjourned for less than thirty days, it is not necessary to give notice of the time and place of the adjourned meeting, other than by announcement at the meeting which is adjourned.");

                Article(col, "7. Methods of holding meetings");
                col.Item().PaddingLeft(10).Text("A meeting of shareholders may be held either — (a) By a number of shareholders who constitute a quorum, being assembled together at the place, date and time appointed for the meeting; or (b) By means of audio, or audio and visual communication by which all shareholders participating and constituting a quorum, can simultaneously hear each other throughout the meeting.");

                Article(col, "8. Quorum");
                Sub(col, "(1)", "Subject to paragraph (3) of this article, no business may be transacted at a meeting of shareholders if a quorum is not present.");
                Sub(col, "(2)", "A quorum for a meeting of shareholders is present if the shareholders or their proxies are present who are between them able to exercise a majority of the votes to be cast on the business to be transacted by the meeting.");
                Sub(col, "(3)", "If a quorum is not present within thirty minutes after the time appointed for the meeting, the meeting is adjourned to the same day in the following week at the same time and place, or to such other date, time and place as the directors may appoint.");

                Article(col, "9. Chairperson");
                Sub(col, "(1)", "If the directors have elected a chairperson of the board, and the chairperson of the board is present at a meeting of shareholders, he or she must chair the meeting.");
                Sub(col, "(2)", "If no chairperson of the board has been elected or if at any meeting of shareholders the chairperson of the board is not present within fifteen minutes of the time appointed for the commencement of the meeting, the shareholders present may choose one of their number to be chairperson of the meeting.");

                Article(col, "10. Voting");
                Sub(col, "(1)", "In the case of a meeting of shareholders held under paragraph (a) of article 7, unless a poll is demanded, voting at the meeting shall be by voice or by show of hands as determined by the chairperson.");
                Sub(col, "(2)", "In the case of a meeting of shareholders held under paragraph (b) of article 7, unless a poll is demanded, voting at the meeting shall be by shareholders signifying individually their assent or dissent by voice.");
                Sub(col, "(3)", "A declaration by the chairperson of the meeting that a resolution is carried by the requisite majority is conclusive evidence of that fact, unless a poll is demanded.");
                Sub(col, "(4)", "At a meeting of shareholders, a poll may be demanded by — (a) Not less than two shareholders having the right to vote at the meeting; or (b) A shareholder or shareholders representing not less than ten per centum of the total voting rights of all shareholders having the right to vote at the meeting.");
                Sub(col, "(5-7)", "A poll may be demanded either before or after the vote is taken on a resolution. If a poll is taken, votes must be counted according to the votes attached to the shares of each shareholder present and voting. The chairperson of a shareholders' meeting is not entitled to a casting vote.");

                Article(col, "11. Proxies");
                Sub(col, "(1)", "A shareholder may exercise the right to vote either by being present in person or by proxy.");
                Sub(col, "(2)", "A proxy for a shareholder is entitled to attend and be heard at a meeting of shareholders as if the proxy were the shareholder.");
                Sub(col, "(3)", "A proxy must be appointed by notice in writing signed by the shareholder. The notice must state whether the appointment is for a particular meeting, or for a specified term.");
                Sub(col, "(4)", "No proxy is effective in relation to a meeting, unless a copy of the notice of appointment is given to the company not less than twenty four hours before the start of the meeting.");

                Article(col, "12. Minutes");
                Sub(col, "(1)", "The board must ensure that minutes are kept of all proceedings at meetings of shareholders.");
                Sub(col, "(2)", "Minutes which have been signed correct by the chairperson of the meeting are prima facie evidence of the proceedings.");

                Article(col, "13. Shareholders proposals");
                col.Item().PaddingLeft(10).Text("Shareholders entitled to do so may give notice of the resolution to the company in accordance with section 142 of this Act and it shall be the duty of the company to give notice of the resolution or circulate the statement, or both, in accordance with section 142.");

                Article(col, "14. Corporations may act by representatives");
                col.Item().PaddingLeft(10).Text("A body corporate which is a shareholder may appoint a representative to attend a meeting of shareholders on its behalf in the same manner as it could appoint a proxy.");

                Article(col, "15. Votes of joint holders");
                col.Item().PaddingLeft(10).Text("Where two or more persons are registered as the holder of a share, the vote of the person named first in the share register and voting on a matter shall be accepted to the exclusion of the votes of the other joint holders.");

                Article(col, "16. Loss of voting right if calls unpaid");
                col.Item().PaddingLeft(10).Text("If a sum due to a company in respect of a share has not been paid, that share may not be voted at a shareholders' meeting other than a meeting of an interest group.");

                Article(col, "17. Annual general meetings and extraordinary general meetings of shareholders");
                Sub(col, "(1)", "Subject to paragraphs (2) and (3) of this article, the board must call an annual meeting of the company — (a) Once in each calendar year; (b) Not later than six months after the balance sheet date; and (c) Not later than fifteen months after the previous annual meeting.");
                Sub(col, "(2)", "The company need not hold its first annual meeting in the calendar year of its incorporation, but must hold that meeting within eighteen months of its incorporation.");
                Sub(col, "(3)", "An extraordinary meeting of shareholders may be called at any time by the board, and must be called by the board on the written request of shareholders holding not less than ten per centum of votes which may be cast on that issue.");
                Sub(col, "(4)", "A resolution in writing signed by not less than fifty per centum of the shareholders entitled to vote, who together hold not less than fifty per centum of the votes entitled to be cast, is as valid as if it had been passed at a meeting of those shareholders.");
                Sub(col, "(5-6)", "Within five working days of a resolution being passed, the company must send a copy to every shareholder who did not sign it. A resolution may be passed without any prior notice being given to shareholders.");

                Article(col, "18. Voting in interest groups");
                col.Item().PaddingLeft(10).Text("Where the company proposes to take action which affects the rights attached to shares within the meaning of section 99 of this Act, the action may not be taken unless it is approved by a special resolution of each interest group, as defined in this Act.");

                Article(col, "19. Shareholders entitled to receive distributions, exercise preemptive rights, and attend and vote at meetings");
                Sub(col, "(1)", "The shareholders who are entitled to receive notice of a meeting of shareholders shall be — (a) If the board fixes a date for the purpose, those shareholders whose names are registered in the share register on that date; (b) If the board does not fix a date, those shareholders whose names are registered in the share register at the close of business on the day immediately preceding the day on which the notice is given.");
                Sub(col, "(2-5)", "A date fixed should not proceed by more than thirty working days the date on which the meeting is to be held. The company may prepare a list of shareholders arranged in alphabetical order before a meeting. A person named in such list is entitled to attend the meeting and vote in respect of the shares shown opposite his name. A shareholder may examine a list during normal business hours at the registered office.");

                // ── SECTION C: DIRECTORS ───────────────────────────────────────
                col.Item().PaddingTop(8).Text("C. DIRECTORS AND SECRETARY").Bold().FontSize(12);
                col.Item().PaddingTop(3).Text("DIRECTORS").Bold().FontSize(11);
                col.Item().PaddingBottom(4).Text("The initial directors of the Company shall be:");

                if (model.Directors != null && model.Directors.Any())
                {
                    foreach (var d in model.Directors)
                    {
                        col.Item().PaddingLeft(15).Text(text =>
                        {
                            text.Span("• ").Bold();
                            text.Span($"{d.Title} {d.FirstNames} {d.Surname}".ToUpper()).Bold();
                            text.Span($" (NIC: {d.NIC}, ADDRESS: {d.Address.ToUpper()}, EMAIL: {d.EmailAddress}, CONTACT: {d.MobileNumber})");
                        });
                    }
                }

                col.Item().PaddingTop(3).Text($"They shall manage the business and affairs of the Company in accordance with the Companies Act No. 7 of 2007 and these Articles.");

                Article(col, "20. Appointment and removal of directors");
                Sub(col, "(1)", "The shareholders may by ordinary resolution fix the number of directors of the company.");
                Sub(col, "(2)", "A director may be appointed or removed by ordinary resolution passed at a meeting called for the purpose or by a written resolution. Unless the company is a private company, the shareholders may only vote on a resolution to appoint a director if the resolution is for the appointment of one director, or a separate resolution has first been passed.");
                Sub(col, "(3)", "A director may resign by delivering a signed written notice of resignation to the registered office of the company. Subject to section 208 of this Act, the notice is effective when it is received at the registered office or at any later time specified in the notice.");
                Sub(col, "(4)", "A director vacates office if he — (a) Resigns; (b) Is removed from office; (c) Becomes disqualified pursuant to section 202; (d) Dies; or (e) Vacates office pursuant to subsection (2) of section 210 on the ground of his age.");

                Article(col, "21. Powers and duties of directors");
                Sub(col, "(1)", "Subject to section 185 of the Act which relates to major transactions, the business and affairs of the company shall be managed by or under the direction or supervision of the board. The board shall have all the powers necessary for managing and for directing and supervising the management of the business and affairs of the company.");
                Sub(col, "(2)", "The board may delegate to a committee of directors or to a director or employee any of its powers which it is permitted to delegate under section 186 of this Act.");
                Sub(col, "(3)", "The directors have the duties set out in the Act, and in particular — (a) Each director must act in good faith and in what he believes to be the best interest of the company; (b) No director shall act or agree to the company to act, in a manner that contravenes any provisions of this Act or these articles.");

                Article(col, "22. Interested directors");
                Sub(col, "(1)", "A director who is interested in a transaction to which the company is a party must disclose that interest in accordance with section 192 of this Act.");
                Sub(col, "(2)", "A director of a company is interested in a transaction to which the company is a party, if, and only if, the director — (a) Is a party to or will or may derive a material financial benefit from the transaction; (b) has a material financial interest in another party to the transaction; (c) Is a director, officer or trustee of another party to the transaction; (d) Is the parent, child or spouse of another party; or (e) Is otherwise directly or indirectly materially interested in the transaction.");
                Sub(col, "(3-9)", "A director of a company is not interested in a transaction where it comprises only the giving of security to a third party with no connection with the director. A director who is interested in a transaction may vote on a matter relating to the transaction, attend meetings, sign documents, and do any other thing in his capacity as a director in relation to the transaction as if he was not interested. A director must disclose all dealings in shares of the company in accordance with sections 198, 199 and 200 of the Act.");

                Article(col, "23. Procedure at meetings of directors");
                Sub(col, "(1-2)", "Articles 24 to 30 sets out the procedure to be followed at meetings of directors. A meeting of directors may determine its own procedure, to the extent that it is not governed by these articles.");

                Article(col, "24. Chairperson");
                Sub(col, "(1)", "The directors may elect one of their numbers to be the chairperson of the board and may determine the period for which the chairperson is to hold office.");
                Sub(col, "(2)", "If no chairperson is elected or if at a meeting of the board the chairperson is not present within five minutes after the time appointed for the commencement of the meeting, the directors present may choose one of their number to be chairperson of the meeting.");

                Article(col, "25. Notice of meeting");
                Sub(col, "(1)", "A director, the secretary or if requested by a director to do so, an employee of the company, may convene a meeting of the board by giving notice in accordance with this article.");
                Sub(col, "(2)", "Not less than twenty-four hours notice of a meeting of the board must be given to every director who is in Sri Lanka.");
                Sub(col, "(3)", "An irregularity in the notice of a meeting is waived if all directors entitled to receive notice of the meeting attend the meeting without protest or if all directors agree to the waiver.");

                Article(col, "26. Methods of holding meetings");
                col.Item().PaddingLeft(10).Text("A meeting of the board may be held either — (a) By a number of the directors who constitute a quorum being assembled together at the place, date and time appointed for the meeting; or (b) By means of audio or audio and visual communication by which all directors participating and constituting a quorum can simultaneously hear each other throughout the meeting.");

                Article(col, "27. Quorum");
                Sub(col, "(1)", "A quorum for a meeting of the board is a majority of the directors.");
                Sub(col, "(2)", "No business may be transacted at a meeting of directors if a quorum is not present.");

                Article(col, "28. Voting");
                Sub(col, "(1-4)", "Every director has one vote. The chairperson has a casting vote. A resolution of the board is passed if it is agreed to by all directors present without dissent or if a majority of the votes cast on it are in favor of it. A director present at a meeting of the board is presumed to have agreed to and voted in favor of a resolution, unless he or she expressly dissents from or votes against the resolution at the meeting.");

                Article(col, "29. Minutes");
                col.Item().PaddingLeft(10).Text("The board must ensure that minutes are kept of all proceedings at meetings of the board.");

                Article(col, "30. Unanimous resolution");
                Sub(col, "(1)", "A resolution in writing signed or assented to by all directors entitled to receive notice of a board meeting, is as valid and effective as if it had been passed at a meeting of the board duly convened and held.");
                Sub(col, "(2)", "Any such resolution may consist of several documents (including facsimile or other similar means of communication) in like form, each signed or assented to by one or more directors.");
                Sub(col, "(3)", "A copy of any such resolution must be entered in the minute book of board proceedings.");

                Article(col, "31. Managing director and other executive directors");
                Sub(col, "(1-6)", "The board may from time to time appoint a director as managing director for such period and on such terms as it thinks fit. Subject to the terms of a managing director's appointment, the board may at any time cancel such appointment. A managing director ceases to hold office if he ceases to be a director of the company. The managing director shall be paid such remuneration as may be agreed between him and the board. The board may delegate powers to the managing director, subject to any conditions or restrictions which they consider appropriate. A director other than the managing director who is employed by the company shall be paid such remuneration as may be agreed between him and the board.");

                Article(col, "32. Secretary");
                Sub(col, "(1-5)", "The company must at all times have a secretary. The board may appoint the secretary for such term and on such conditions as it thinks fit. The board may remove the secretary. The secretary may not be the sole director of the company, or a corporation whose sole director is the sole director of the company. Where the Act or these articles require something to be done by a director and the secretary, it is not satisfied by the same person doing that thing acting in both capacities.");

                // ── SECTION D ─────────────────────────────────────────────────
                col.Item().PaddingTop(8).Text("D. SHAREHOLDER RIGHTS").Bold().FontSize(12);

                Article(col, "33. Rights attached to shares");
                Sub(col, "(1)", "Each share confers on the holder the right to — (a) Receive notice of, attend, and speak at general meetings of the company; (b) Vote at General meetings in accordance with paragraph (4) of article 17; (c) Share equally in any dividends or other distributions declared by the company; (d) Share equally in the distribution of surplus assets on a winding up of the company.");

                Article(col, "34. Voting");
                Sub(col, "(1-4)", "Each shareholder has one vote. No alteration shall be made to these Articles or to the rights of any class of shares which would disproportionately disadvantage any minority shareholder without their consent. Shareholders holding not less than ten per centum of the issued shares may requisition a general meeting. Each shareholder's vote shall carry the same weight per share held, and the vote of a minority shareholder shall be as valid and binding as the vote of any other shareholder. No resolution or decision shall disregard, invalidate, or diminish a minority shareholder's voting rights solely by reason of their minority status.");

                // ── SECTION E ─────────────────────────────────────────────────
                col.Item().PaddingTop(8).Text("E. ACCOUNTS AND AUDIT").Bold().FontSize(12);

                Article(col, "35. Accounting records, financial statements, audit etc.");
                Sub(col, "(1)", "The board must ensure that the company keeps accounting records which — (a) Correctly record and explain the company's transactions; (b) Will at any time enable the financial position of the company to be determined with reasonable accuracy; (c) Will enable the board to prepare financial statements; and (d) Will enable the financial statements of the company to be readily and properly audited.");
                Sub(col, "(2)", "The accounting records must comply with subsection (2) of section 148 of this Act.");
                Sub(col, "(3)", "The board shall ensure that within five months after the balance sheet date of the company, financial statements which comply with section 151 of the Act are completed in relation to that balance sheet date and are dated and signed on behalf of the board by two directors.");
                Sub(col, "(4)", "At every annual meeting, the company must appoint an auditor for the following year in accordance with section 154 of the Act. An auditor who is appointed at an annual meeting is deemed to be reappointed at the following annual meeting, unless — (a) He is not qualified for re-appointment; (b) The company resolves at that meeting to appoint another person in his place; or (c) The auditor has given notice to the company that he does not wish to be re-appointed.");
                Sub(col, "(5)", "The board must within five months after the balance sheet date of the company, prepare an annual report on the affairs of the company during the accounting period ending on that date. The board must send a copy of the annual report to every shareholder not less than twenty working days before the date fixed for holding the annual meeting of shareholders.");

                // ── SECTION F ─────────────────────────────────────────────────
                col.Item().PaddingTop(8).Text("F. LIQUIDATION AND REMOVAL FROM THE REGISTER").Bold().FontSize(12);

                Article(col, "36. Resolution to appoint liquidator");
                col.Item().PaddingLeft(10).Text("The shareholders may resolve to wind up the company voluntarily by special resolution.");

                Article(col, "37. Distribution of surplus assets");
                Sub(col, "(1)", "The surplus assets of the company available for distribution to shareholders after all creditors of the company have been paid, shall be distributed in proportion to the number of shares held by each shareholder, subject to the terms of issue of any shares.");
                Sub(col, "(2)", "The liquidator may with the approval of a special resolution, divide the surplus assets of the company among the shareholders in kind. For this purpose, he may set such value as he considers fair on any property to be divided, and may determine how the division will be carried out as between the shareholders or different classes of shareholders.");

                // ── SECTION G ─────────────────────────────────────────────────
                col.Item().PaddingTop(8).Text("G. MISCELLANEOUS").Bold().FontSize(12);

                Article(col, "38. Documents to be kept by company");
                Sub(col, "(1)", "The company must keep at its registered office the following documents — (a) The certificate of incorporation and the articles of the company; (b) Minutes of all meetings and resolutions of shareholders within the last ten years; (c) An interests register; (d) Minutes of all meetings and resolutions of directors within the last ten years; (e) Certificates given by directors under this Act within the last ten years; (f) The register of directors and secretaries; (g) Copies of all written communication to all shareholders during the last ten years, including annual reports; (h) Copies of all financial statements for the last ten completed accounting periods; (i) The copies of instruments creating or evidencing charges and the register of charges; (j) The share register; and (k) The accounting records for the current accounting period and for the last ten completed accounting periods.");
                Sub(col, "(2)", "The references in paragraph (1) of this article to \"ten years\" and to \"ten completed accounting periods\" shall include such lesser periods as the Registrar may approve, by notice in writing to the company.");

                Article(col, "39. Rights of directors and shareholders to documents etc.");
                Sub(col, "(1)", "The directors of the company are entitled to have access to the company's records in accordance with section 118 of the Act.");
                Sub(col, "(2)", "A shareholder of the company is entitled — (a) To inspect the documents referred to in section 119 of the Act, in the manner specified in section 121 of the Act; and (b) To require copies of or extracts from any document which he may inspect, within five working days of making a request in writing, on payment of any reasonable copying and administration fee determined by the company.");

                Article(col, "40. Name of company");
                col.Item().PaddingLeft(10).Text("The company may change its name by special resolution in accordance with section 8 of the Act.");

                Article(col, "41. Notices");
                Sub(col, "(1)", "Where the company is required to send any document to a shareholder or to give notice of any matter to a shareholder, it shall be sufficient for the company to send the document or notice to the registered address of the shareholder by ordinary post. Any document or notice so sent is deemed to have been received by the shareholder within three working days of the posting of a properly addressed and prepaid letter.");
                Sub(col, "(2)", "A shareholder whose registered address is outside Sri Lanka may give notice to the company of an address in Sri Lanka to which all documents and notices are to be sent, and the company shall treat that address as the registered address of the shareholder for all purposes.");
                Sub(col, "(3-5)", "A document may be sent or notice given to the joint holders of a share, by giving the notice to the holder first named on the share register. Where a shareholder has died or has become bankrupt or insolvent, the company may continue to send all notices and documents in respect of his shares addressed to him at his registered address. A copy of every notice or document sent to all shareholders must be sent to the auditor of the company.");

                Article(col, "42. Insurance and indemnity");
                Sub(col, "(1)", "The company shall indemnify every director, auditor and secretary of the company for the time being against any costs incurred in the course of defending any proceeding that relates to any act or omission in his capacity as director, auditor or secretary, in which judgment is given in his favor or in which he is acquitted or which is discontinued.");
                Sub(col, "(2)", "The company may indemnify a director or employee in circumstances where paragraph (1) does not apply, to the extent permitted by subsection (3) of section 218 of the Act, if the board considers it appropriate to do so.");

                Article(col, "43. Private Company");
                Sub(col, "(1)", "The Company will be registered as a private company, under the provisions of Section 27 of the Companies Act.");
                Sub(col, "(2)", "The company must not offer any shares or other securities issued by it to the public.");
                Sub(col, "(3)", "The company must at no time have more than fifty shareholders, not including shareholders who are — (a) Employees of the company; or (b) Former employees who became shareholders of the company while being employed by it, and who have continued to be shareholders after ceasing employment with the company.");
                Sub(col, "(4)", "The company may by unanimous resolution of its shareholders dispense with the keeping of an interests register. Any such resolution shall cease to have effect if any shareholder gives notice in writing to the company that he requires it to keep an interest register.");
                Sub(col, "(5)", "Where all the shareholders of the company agree to or concur in any action which has been taken or is to be taken by the company — (a) The taking of that action is deemed to be validly authorized by the company, notwithstanding anything in these articles; And (b) The provisions of this Act referred to in the Second Schedule to this Act, do not apply in relation to that action, pursuant to section 31 of the Act.");

                Article(col, "44. Interpretation");
                col.Item().PaddingLeft(10).Text("In these articles \"the Act\" means the Companies Act, No. 07 of 2007, and terms which are defined in the Act, shall have the same meaning in these articles.");

                // ── SIGNATURE SECTION ─────────────────────────────────────────
                col.Item().PaddingTop(16).Text("We the initial shareholders of the proposed Company hereby agree to the foregoing Articles of Association.").Bold();

                int sigNum = 1;
                foreach (var sub in subscribers)
                {
                    col.Item().PaddingTop(12).Column(sig =>
                    {
                        sig.Item().Text($"{sigNum}. {sub.Name}").Bold();
                        sig.Item().Text($"NIC / BR: {sub.NicOrBr}");
                        sig.Item().Text($"Address: {sub.Address}");
                        sig.Item().PaddingTop(6).Row(row =>
                        {
                            row.RelativeItem().Text("Signature: ");
                            row.RelativeItem(3).BorderBottom(1).MinHeight(20).Text("");
                        });
                    });
                    sigNum++;
                }

                col.Item().PaddingTop(20).Text($"At {model.District ?? "Colombo"} on this {today} to the foregoing.");
            });
        }

        // ── HELPERS ──────────────────────────────────────────────────────────
        private void Article(ColumnDescriptor col, string title)
        {
            col.Item().PaddingTop(6).Text(title).Bold();
        }

        private void Sub(ColumnDescriptor col, string num, string text)
        {
            col.Item().PaddingLeft(12).Text($"{num}  {text}");
        }
    }
}
