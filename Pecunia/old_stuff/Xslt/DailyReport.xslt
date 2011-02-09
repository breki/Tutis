<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:template match="/">
        <!--<link rel="stylesheet" href="..\xml\Criticality.css" type="text/css" />-->
        <html>
            <body>
                <xsl:for-each select="Reports/Report">
                    <div>
                        <img src="{Image}" />
                    </div>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>

</xsl:stylesheet>
